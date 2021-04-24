using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script contains every single action Sonic is capable. They should all be derived from ActionBase.
/// Every action script needs to have Initialize, Update, and FixedUpdate override functions, as well as
/// a line to initialize the base class. It is not necessary to call base.FixedUpdate or base.Update, only base.Initialize.
/// </summary>
[System.Serializable]
public class ActionBase
{
    [HideInInspector] public PlayerController player;
    [HideInInspector] public PlayerActions actions;
    /// <summary>
    /// In child action classes, Initialize is used to set the Player and Action values if they have not already been set,
    /// as well as any other things you wish to perform upon changing actions, such as adding the initial jump force, or
    /// beginning the spin dash charge.
    /// </summary>
    public virtual void InitializeState(ActionBase _base) {
        if (!player || !actions)
        {
            player = _base.player;
            actions = _base.actions;
        }
    }
    /// <summary>
    /// UpdateState and FixedUpdateState are more or less normal Update and FixedUpdate functions, except they are only
    /// updated if that state is the current active state.
    /// </summary>
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
}
[System.Serializable]
public class ActionDefault : ActionBase
{
    public float RollingStartSpeed;
    public CapsuleCollider col;
    public float CrouchHeight = 0.67f; //How tall the collider is when crouching
    public float CrouchOffset = -0.028f; //Y Offset of the collider when crouching
    public bool DidDash { get; set; }
    public override void InitializeState(ActionBase _base)
    {
        base.InitializeState(_base);
        Debug.Log("Current State: Regular");
    }

    public override void UpdateState()
    {
        float speed = player.rigidBody.velocity.magnitude;
        if (player.Grounded)
        {
            DidDash = false;
            if (player.a_input.GetButton("Jump", InputHandler.ButtonState.Down))
            {
                bool JumpDisabled = player.Crouching && actions.spinDashState.spinDashType == ActionSpindash.DashStyle.Classic;
                if (!JumpDisabled)
                {
                    actions.animator.PlayJumpSound();
                    actions.animator.PlayJumpVoice();
                    actions.ChangeState(1);
                }
            }
            if (player.a_input.GetButton("Crouch", InputHandler.ButtonState.Down) && speed >= RollingStartSpeed)
            {
                player.Crouching = !player.Crouching;
            }
            if (speed < RollingStartSpeed)
            {
                if (actions.spinDashState.spinDashType == ActionSpindash.DashStyle.Classic)
                {
                    player.Crouching = player.a_input.GetButton("Crouch", InputHandler.ButtonState.Get);
                    if (player.Crouching && player.a_input.GetButton("Jump", InputHandler.ButtonState.Down))
                    {

                        col.height = CrouchHeight;
                        col.center = new Vector3(0, CrouchOffset, 0);
                        actions.ChangeState(4);
                    }
                } else if (actions.spinDashState.spinDashType == ActionSpindash.DashStyle.Adventure)
                {
                    if (player.a_input.GetButton("Crouch", InputHandler.ButtonState.Down))
                    {
                        col.height = CrouchHeight;
                        col.center = new Vector3(0, CrouchOffset, 0);
                        actions.ChangeState(4);
                    }
                }
            }

            col.height = player.Crouching ? CrouchHeight : 1f;
            Vector3 Offset = new Vector3(0, player.Crouching ? CrouchOffset : 0f, 0);
            col.center = Offset;

            actions.homingState.OnExit();

        } else
        {
            if (player.a_input.GetButton("Jump", InputHandler.ButtonState.Down))
            {
                if (actions.ClosestTarget != null)
                {
                    actions.ChangeState(2);
                } else
                {
                    if (!DidDash)
                    {
                        DidDash = true;
                        if (actions.jumpState.OnJumpAction == ActionJump.JumpAction.DoubleJump)
                        {
                            actions.jumpState.JumpIndex = 1;
                            actions.animator.PlayDoubleJumpSound();
                            actions.ChangeState(1);
                        } else if (actions.jumpState.OnJumpAction == ActionJump.JumpAction.AirDash)
                        {
                            actions.ChangeState(2);
                        }
                    }
                }
            }
        }

        actions.UpdateTargets();
    }
    public override void FixedUpdateState()
    {

    }
}
[System.Serializable]
public class ActionJump : ActionBase
{
    public enum JumpAction { DoubleJump, AirDash }
    public JumpAction OnJumpAction;
    public float InitialJumpForce;
    public float JumpSpeed;
    public float MaxJumpTime;
    public float MinJumpTime;
    public int MaxJumps;
    public int JumpIndex { get; set; }
    float jumpTimer;
    public override void InitializeState(ActionBase _base)
    {
        base.InitializeState(_base);
        Debug.Log("Current State: Jump");
        jumpTimer = 0;
        player.Grounded = false;
        player.rigidBody.position += player.GroundNormal * player.GroundCheckDistance;
        player.rigidBody.AddForce(player.GroundNormal * InitialJumpForce, ForceMode.Impulse);
        if (OnJumpAction == JumpAction.DoubleJump)
        {
            JumpIndex++;
            if (JumpIndex > 2)
                actions.animator.PlayDoubleJumpSound();
        }
    }

    public override void UpdateState()
    {
        actions.UpdateTargets();
        if (jumpTimer < MaxJumpTime)
            jumpTimer += Time.deltaTime;
        if (player.a_input.GetButton("Jump", InputHandler.ButtonState.Up) && jumpTimer < MaxJumpTime)
            jumpTimer = MaxJumpTime;
        if (player.Grounded && jumpTimer > MinJumpTime)
        {
            JumpIndex = 0;
            actions.ChangeState(0);
            actions.homingState.OnExit();
        }

        if (player.a_input.GetButton("Jump", InputHandler.ButtonState.Down))
        {
            if (actions.ClosestTarget == null)
            {
                if (OnJumpAction == JumpAction.DoubleJump && JumpIndex < MaxJumps)
                {
                    player.rigidBody.velocity = Vector3.ProjectOnPlane(player.rigidBody.velocity, player.GroundNormal);
                    InitializeState(null);
                }
                if (OnJumpAction == JumpAction.AirDash)
                {
                    JumpIndex = 0;
                    player.rigidBody.velocity = Vector3.zero;
                    actions.ChangeState(2);
                }
            }
            else
            {
                actions.ChangeState(2);
            }

        }
    }
    public override void FixedUpdateState()
    {
        if (jumpTimer < MaxJumpTime)
            player.rigidBody.velocity += player.GroundNormal * JumpSpeed;
    }
}
[System.Serializable]
public class ActionHoming : ActionBase
{
    public RectTransform ReticleTransform;
    public Animator ReticleAnimator;
    public GameObject Reticle;
    public RectTransform Hud;
    public float MaxHomingDistance;
    public LayerMask TargetLayer;
    public LayerMask BlockingLayers;
    [Range(0, 1)] public float FieldOfView;
    public VolumeTrailRenderer HomingTrail;

    [Header("Air Dash")]
    public bool OverrideSpeed;
    public float AirDashSpeed;
    public float AirDashDuration;
    [Range(0, 1)] public float AirDashReleaseVelocity;
    float DashSpeed;

    [Header("Homing")]
    public float HomingTimeOut = 2f;
    public float HomingSpeed;
    [Range(0, 1)] public float HomingReleaseVelocity;
    public float HomingBouncePower;

    float t;

    public Transform ActiveTarget { get; set; }
    public override void InitializeState(ActionBase _base)
    {
        base.InitializeState(_base);
        Debug.Log("Current State: Homing");
        //if (actions.ClosestTarget != null)
        //ActiveTarget = actions.ClosestTarget;
        if (ActiveTarget != null)
            actions.animator.PlayHomingVoice();
        t = 0;
        actions.animator.PlayHomingSound();

        if (!OverrideSpeed && player.rigidBody.velocity.magnitude > AirDashSpeed)
            DashSpeed = player.rigidBody.velocity.magnitude;
        else
            DashSpeed = AirDashSpeed;
        HomingTrail.emitTime = AirDashDuration;
        HomingTrail.emit = true;
        actions.defaultState.DidDash = true;
    }

    public override void UpdateState()
    {

    }
    public override void FixedUpdateState()
    {
        t += Time.fixedDeltaTime;
        if (ActiveTarget != null)
        {
            //Get direction to target
            Vector3 Direction = Vector3.Normalize(ActiveTarget.position - player.transform.position);
            player.rigidBody.velocity = Direction * HomingSpeed;
            player.rigidBody.position = Vector3.MoveTowards(player.rigidBody.position, ActiveTarget.position, HomingSpeed * Time.fixedDeltaTime);

        }
        else
        {
            if (t < AirDashDuration)
            {
                Vector3 DashDirection = Vector3.ProjectOnPlane(player.transform.forward, player.GroundNormal);
                player.rigidBody.velocity = DashDirection * DashSpeed;
            }
            else
            {
                //Release air dash
                player.rigidBody.velocity *= AirDashReleaseVelocity;
                if (player.Crouching)
                    player.Crouching = false;
                actions.ChangeState(0);
            }
        }
    }
    /// <summary>
    /// Updates the state while in other states.
    /// </summary>
    /// BackgroundUpdate on Homing State is meant to set the current homing target as well as handle the homing reticle.
    public void BackgroundUpdate()
    {
        if (!actions || !player)
        {
            //Set the Player and Actions values here since this is running in the background and normally they wouldn't be set until Initialize is called
            actions = GameObject.FindObjectOfType<PlayerActions>();
            player = GameObject.FindObjectOfType<PlayerController>();
        }

        //This is going to be used for setting the reticle's position and animation, among other things.
        if (actions.ClosestTarget != null && ActiveTarget != actions.ClosestTarget && !player.Grounded)
        {
            if (!Reticle.activeSelf)
                Reticle.SetActive(true);
            ReticleAnimator.SetTrigger("Target");
            actions.animator.PlayTargetSound();
            ActiveTarget = actions.ClosestTarget;
        }
        if (actions.ClosestTarget == null)
        {
            ActiveTarget = null;
        }
        if (actions.ClosestTarget == null || ActiveTarget == null)
        {
            Reticle.SetActive(false);
        }

        if (Reticle.activeSelf)
        {
            ///For setting the reticle's position, we simply convert the target's screen position into canvas space.
            Vector2 IconPos = Camera.main.WorldToScreenPoint(ActiveTarget.position);
            Vector2 CanvasPos = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Hud, IconPos, null, out CanvasPos);
            ReticleTransform.localPosition = CanvasPos;
        }

    }

    public void OnExit()
    {
        if (ActiveTarget != null)
            ActiveTarget = null;
    }
}

[System.Serializable]
public class ActionSpindash : ActionBase
{
    public CapsuleCollider col;
    public enum DashStyle { Classic, Adventure }
    public DashStyle spinDashType;

    public int MaxRevs;
    public float MaxCharge;
    public float ChargeDuration; //How long does it take to reach max charge
    public float SpinDashCharge { get; set; }
    float RevAmount;

    public bool ChargeWhileRunning;
    public override void InitializeState(ActionBase _base)
    {
        base.InitializeState(_base);
        Debug.Log("Current State: Spin Dash");
        if (player != null)
            Debug.Log("Found Player Controller");
        if (actions != null)
            Debug.Log("Found Action Controller");
        col.height = actions.defaultState.CrouchHeight;
        col.center = new Vector3(0, actions.defaultState.CrouchOffset, 0);

        SpinDashCharge = 0;
        RevAmount = MaxCharge / MaxRevs;
        switch (spinDashType)
        {
            case DashStyle.Adventure:
                actions.animator.SpinDashLoop();
                break;
            case DashStyle.Classic:
                actions.animator.SpinDashSingle();
                break;
        }
    }
    public override void UpdateState()
    {
        base.UpdateState();
        if (SpinDashCharge < MaxCharge)
        {
            switch (spinDashType)
            {
                case DashStyle.Adventure:
                    SpinDashCharge += MaxCharge * (Time.deltaTime / ChargeDuration);
                    break;
                case DashStyle.Classic:
                    if (player.a_input.GetButton("Jump", InputHandler.ButtonState.Down))
                    {
                        SpinDashCharge += RevAmount;
                        actions.animator.SpinDashSingle();
                    }
                    break;
            }
        }
        if (player.a_input.GetButton("Crouch", InputHandler.ButtonState.Up))
        {
            actions.animator.SpinDashRelease();
            player.Crouching = true;
            player.rigidBody.velocity = player.transform.forward * SpinDashCharge;
            actions.ChangeState(0);
        }
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        player.rigidBody.velocity = Vector3.zero;
    }
}

[System.Serializable] public class ActionHurt : ActionBase
{
    public float PositionOffset = 0.05f;
    public float BackForce; //How far back is Sonic bumped when hitting an enemy or hazard
    public float UpForce; //How high is Sonic bumped when hitting an enemy or hazard
    public float StateChangeDelay; //How long does Sonic stay on the ground before getting up
    public float RespawnDelay; //How long does Sonic stay on the ground before respawning
    float t;
    public override void InitializeState(ActionBase _base)
    {
        base.InitializeState(_base);
        if (!PlayerHealth.IsDead)
        {
            t = StateChangeDelay;
        } else
        {
            t = RespawnDelay;
        }
        //Lock Sonic's input and rotation
        player.InputLocked = true;
        player.RotationLocked = true;
        player.rigidBody.position += player.GroundNormal * (player.GroundCheckDistance + PositionOffset);
        player.rigidBody.velocity = -player.transform.forward * actions.hurtState.BackForce + Vector3.up * actions.hurtState.UpForce;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (player.Grounded)
        {
            player.rigidBody.velocity = Vector3.zero;
            //Start the timer, and unlock input and change states when the timer is up
            t -= Time.deltaTime;
            if (t <= 0 && !PlayerHealth.IsDead)
            {
                    player.InputLocked = false;
                    player.RotationLocked = false;
                    actions.ChangeState(0);
            }
            if (PlayerHealth.IsDead)
            {
                PlayerHealth _health = player.GetComponent<PlayerHealth>();
                if (_health.LifeCount > 0)
                {
                    StageProgress.instance.BeginRespawn();
                }
                else
                {
                    StageProgress.instance.BeginGameOver();
                }
                actions.ChangeState(0);
            }
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}
