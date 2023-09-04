
using UnityEngine;
[AddComponentMenu("Pinball/Actions/Default")]
[RequireComponent(typeof(PlayerActions))]
public class DefaultState : ActionBase
{
    public float RollingStartThreshold;
    public float SpindashStartThreshold;
    public CapsuleCollider col;
    private bool hasCrouched = false;
    public float CrouchHeight = 0.67f; //How tall the collider is when crouching
    public float CrouchOffset = -0.028f; //Y Offset of the collider when crouching
    public override void InitializeState(PlayerController p, PlayerActions a)
    {
        base.InitializeState(p, a);
        actions.StateIndex = 0;
        Debug.Log("Current State: Regular");
    }
    
    public override void UpdateState()
    {
        
        float speed = player.rigidBody.velocity.magnitude;
        if (player.Crouching && !hasCrouched)
        {
            actions.animator.PlayCrouchSound();
            if (speed > 5f)
            {
                actions.animator.HomingTrail.emitTime = 10f;
                actions.animator.HomingTrail.emit = true;
            }
            

            hasCrouched = true;
        }
        if (!player.Crouching && hasCrouched)
        {
            actions.animator.HomingTrail.emit = false;

            hasCrouched = false;
        }
        if (player.Grounded)
        {
            actions.DidDash = false;
            if (player.p_input.GetButtonDown("Jump"))
            {
                bool JumpDisabled = player.Crouching && player.rigidBody.velocity.magnitude < RollingStartThreshold;
                if (!JumpDisabled)
                {
                    actions.animator.PlayJumpSound();
                    actions.animator.PlayJumpVoice();
                    if (actions.CheckForState(typeof(JumpState))) actions.ChangeState(typeof(JumpState));
                }
            }
            if (((player.p_input.GetAxis("Roll") < SpindashStartThreshold) && player.p_input.GetAxis("Roll") > RollingStartThreshold) || player.p_input.GetButtonDown("RollDigital"))
            {
                
                player.Crouching = true;
            }
            if (player.p_input.GetAxis("Roll") <= RollingStartThreshold && !player.p_input.GetButton("RollDigital"))
            {
                player.Crouching = false;
            }

                if (player.p_input.GetAxis("Roll") >= SpindashStartThreshold || player.p_input.GetButtonDown("SpinDash"))
                {
                    col.height = CrouchHeight;
                    col.center = new Vector3(0, CrouchOffset, 0);
                    actions.ChangeState(typeof(SpinDashState));
                }

            col.height = player.Crouching ? CrouchHeight : 1f;
            Vector3 Offset = new Vector3(0, player.Crouching ? CrouchOffset : 0f, 0);
            col.center = Offset;

            //actions.homingState.OnExit();

        }
        else
        {
            if (player.p_input.GetButtonDown("Roll") || player.p_input.GetButtonDown("SpinDash"))
            {
                if (actions.CheckForState(typeof(StompState)))
                {
                    actions.ChangeState(typeof(StompState));
                }
            }
            if (player.p_input.GetButtonDown("Jump"))
            {
                if (actions.ClosestTarget == null)
                {
                    if (actions.CheckForState(typeof(DoubleJumpState)))
                    {
                        actions.ChangeState(typeof(DoubleJumpState));
                    }
                    else
                    {
                        if (actions.CheckForState(typeof(HomingState)) && !actions.DidDash)
                        {
                            actions.DidDash = true;
                            actions.ChangeState(typeof(HomingState));
                        }
                    }
                }
                else
                {
                    if (actions.CheckForState(typeof(HomingState)))
                    {
                        actions.ChangeState(typeof(HomingState));
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
