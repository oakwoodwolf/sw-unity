using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorInteraction : MonoBehaviour
{
    ObjectPool pool;
    ScoreController score;
    StageProgress stageProgress;
    PlayerController Player;
    PlayerActions actions;
    PlayerAnimator anim;
    PlayerHealth _health;
    public PlayerCamera camera;
    public int EnemyScore = 100;
    [SerializeField] Object ringParticles;
    public Transform PulleyGripPoint;
    PulleyActor currentPulley;

    MovingPlatform _platform;

    private void Start()
    {
        anim = GetComponent<PlayerAnimator>();
        Player = GetComponent<PlayerController>();
        actions = GetComponent<PlayerActions>();
        stageProgress = FindObjectOfType<StageProgress>();
        _health = GetComponent<PlayerHealth>();
        pool = ObjectPool.PoolManager;
        score = ScoreController._score;
    }

    private void Update()
    {
        if (_platform != null)
        {
            StartCoroutine("PlatformMovement");
        }
        if (currentPulley != null)
        {
            actions.UpdateTargets();
            Player.rigidBody.velocity = Vector3.zero;
            Player.InputLocked = true;
            Vector3 HandPos = transform.position - PulleyGripPoint.position;
            transform.position = currentPulley.HandleGripPos.position + HandPos;
            transform.forward = -currentPulley.transform.forward;
            if (!currentPulley.Moving)
            {
                actions.ChangeState(0);
                Player.InputLocked = false;
                Player.rigidBody.velocity += currentPulley.transform.up * (currentPulley.Speed * currentPulley.MaxLength);
                currentPulley = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (stageProgress == null)
        {
            Debug.LogError("Stage Progress object not found. Please add a Stage Progress object.");
            return;
        }
        #region Items
        if (other.GetComponent<RingActor>())
        {
            RingActor ring = other.GetComponent<RingActor>();
            pool.SpawnFromPool("RingParticles", ring.transform.position, Quaternion.identity);
            anim.PlayRingSound(false);
            ring.gameObject.SetActive(false);
            PlayerHealth.RingCount++;
            _health.RingsToLife--;
        }
        if (other.GetComponent<ItemCapsuleActor>())
        {
            ItemCapsuleActor cap = other.GetComponent<ItemCapsuleActor>();
            if (cap.IsActive)
            {
                anim.PlayCapsuleSound();
                //Add items
                switch (cap.capItem)
                {
                    case ItemCapsuleActor.Item.TenRings:
                        StartCoroutine(AddRings(10, 0.5f));
                        break;
                    case ItemCapsuleActor.Item.FiveRings:
                        StartCoroutine(AddRings(5, 0.5f));
                        break;
                    case ItemCapsuleActor.Item.Shield:
                        StartCoroutine(AddShield(0, 0.5f));
                        break;
                    case ItemCapsuleActor.Item.Invincible:
                        StartCoroutine(AddShield(1, 0.5f));
                        break;
                    case ItemCapsuleActor.Item.OneUp:
                        StartCoroutine(AddLife(0.5f));
                        break;
                }
                cap.IsActive = false;
                if (PlayerActions.currentState is ActionJump)
                {
                    if (Player.rigidBody.velocity.y < 0)
                        Player.rigidBody.velocity = Vector3.Reflect(Player.rigidBody.velocity, cap.transform.up);
                } else if (PlayerActions.currentState is ActionHoming)
                {
                    Player.rigidBody.velocity = Vector3.ProjectOnPlane(Player.rigidBody.velocity, Player.GroundNormal) * actions.homingState.HomingReleaseVelocity;
                    Player.rigidBody.velocity += transform.up * actions.homingState.HomingBouncePower;
                    actions.ChangeState(1);
                }

                actions.UpdateTargets();
            }
        }
        #endregion
        #region Boosters
        if (other.GetComponent<SpringActor>())
        {
            SpringActor spring = other.GetComponent<SpringActor>();
            transform.position = spring.transform.position + spring.transform.up * spring.PositionOffset;
            actions.ChangeState(0);
            Player.TransformNormal = spring.transform.up;
            Player.rigidBody.velocity = spring.transform.up * spring.SpringForce;
            actions.defaultState.DidDash = false;
            if (Player.Crouching) Player.Crouching = false;
            Player.StartCoroutine(Player.LockInput(spring.LockDuration));
        }

        if (other.GetComponent<DashPanelActor>())
        {
            DashPanelActor dashPanel = other.GetComponent<DashPanelActor>();
            if (dashPanel.SnapToPosition)
                Player.rigidBody.position = dashPanel.transform.position + dashPanel.transform.up * 0.5f;
            if (dashPanel.IsAdditive)
                Player.rigidBody.velocity = dashPanel.transform.forward * (Player.rigidBody.velocity.magnitude + dashPanel.Force);
            else
                Player.rigidBody.velocity = dashPanel.transform.forward * dashPanel.Force;
            Player.StartCoroutine(Player.LockInput(dashPanel.LockDuration));
        }
        #endregion
        #region Stage Objects
        if (other.gameObject.CompareTag("PulleyHandle"))
        {
            currentPulley = other.gameObject.GetComponentInParent<PulleyActor>();
            actions.ChangeState(5);
            currentPulley.RetractPulley();
        }
        #endregion
        #region Hazards and Enemies
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (PlayerActions.currentState is ActionHoming || PlayerActions.currentState is ActionJump)
            {
                pool.SpawnFromPool("HitExplosion", other.transform.position, Quaternion.identity);
                other.gameObject.SetActive(false);
                if (PlayerActions.currentState is ActionHoming)
                {
                    Player.rigidBody.velocity = Vector3.ProjectOnPlane(Player.rigidBody.velocity, Player.GroundNormal) * actions.homingState.HomingReleaseVelocity;
                    Player.rigidBody.velocity += transform.up * actions.homingState.HomingBouncePower;
                    actions.ChangeState(1);
                } else if (PlayerActions.currentState is ActionJump)
                {
                    if (Player.rigidBody.velocity.y < 0)
                        Player.rigidBody.velocity = Vector3.Reflect(Player.rigidBody.velocity, other.transform.up);
                }

                anim.PlayEnemyHit();
                score.AddScore(EnemyScore);
            } else if (PlayerActions.currentState is ActionDefault)
            {
                ///If we are grounded and rolling or have an invincible shield, destroy the enemy. Otherwise, take damage.
                if (Player.rigidBody.velocity.magnitude >= actions.defaultState.RollingStartSpeed && Player.Crouching && Player.Grounded ||
                    _health.HasShield && _health.IsInvincible)
                {
                    pool.SpawnFromPool("HitExplosion", other.transform.position, Quaternion.identity);
                    other.gameObject.SetActive(false);
                    anim.PlayEnemyHit();
                    score.AddScore(EnemyScore);
                } else
                {
                    if (!_health.IsInvincible)
                    {
                        _health.TakeDamage();
                        Vector3 Direction = transform.position - other.gameObject.transform.position;
                        Player.Grounded = false;
                        transform.forward = -Vector3.ProjectOnPlane(Direction, Vector3.up).normalized;
                        actions.ChangeState(3);
                    }
                }
            }
        }

        if (other.gameObject.CompareTag("Hazard"))
        {
            if (!_health.IsInvincible)
            {
                _health.TakeDamage();
                Vector3 Direction = transform.position - other.gameObject.transform.position;
                Player.Grounded = false;
                transform.forward = -Vector3.ProjectOnPlane(Direction, Vector3.up).normalized;
                actions.ChangeState(3);
            }
        }

        if (other.gameObject.CompareTag("KillPlane"))
        {
            camera.UpdateCameraOrientation = false;
            camera.camState = PlayerCamera.CameraState.Fixed;
            camera.TargetPosition = camera.transform.position;
            anim.PlayDeathVoice();
            _health.RemoveShield();
            if (_health.LifeCount > 0)
            {
                StageProgress.instance.BeginRespawn();
            }
            else
            {
                StageProgress.instance.BeginGameOver();
            }
        }

        #endregion
        #region Stage Progress
        if (other.GetComponent<CheckpointActor>())
        {
            CheckpointActor check = other.GetComponent<CheckpointActor>();
            if (!check.IsActive)
            {
                check.IsActive = true;
                stageProgress.SpawnPosition = check.transform.position;
                stageProgress.SpawnRotation = check.transform.rotation;
                check.GetComponent<AudioSource>().Play();
            }
        }

        #endregion
    }

    private void OnTriggerStay(Collider other)
    {
        //Camera Triggers
        if (other.GetComponent<CameraTrigger>())
        {
            CameraTrigger trig = other.GetComponent<CameraTrigger>();
            camera.UpdateCameraOrientation = false;
            int NextCameraState = (int)trig.camState;
            camera.camState = (PlayerCamera.CameraState)NextCameraState;
            if (NextCameraState == 2)
            {
                camera.TargetPosition = trig.TargetPosition.position;
            }

            if (trig.ChangeDirection)
            {
                camera.SetLookDir(trig.Direction);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CameraTrigger>())
        {
            CameraTrigger trig = other.GetComponent<CameraTrigger>();
            if (trig.ResetOnExit)
            {
                camera.camState = PlayerCamera.CameraState.Auto;
            }
            camera.UpdateCameraOrientation = true;
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Platform"))
        {
            _platform = col.transform.parent.GetComponent<MovingPlatform>();
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Platform"))
        {
            _platform = null;
        }
    }
    IEnumerator AddRings(int amount, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayerHealth.RingCount += amount;
        _health.RingsToLife -= amount;
        anim.ObjectSoundSource.PlayOneShot(anim.GetSoundFromBank("RingCapsule", anim.ObjectSounds));
    }

    IEnumerator AddShield(int shieldType, float delay)
    {
        yield return new WaitForSeconds(delay);
        _health.AddShield(shieldType);
        if (shieldType == 0)
            anim.ObjectSoundSource.PlayOneShot(anim.GetSoundFromBank("Shield", anim.ObjectSounds));
    }

    IEnumerator AddLife(float delay)
    {
        yield return new WaitForSeconds(delay);
        _health.LifeCount++;
        anim.PlayOneUp();
    }

    IEnumerator PlatformMovement()
    {
        yield return new WaitForFixedUpdate();
        Player.rigidBody.position += _platform.platform.velocity * Time.fixedDeltaTime;
    }
}
