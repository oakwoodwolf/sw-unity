
using UnityEngine;
[AddComponentMenu("Pinball/Actions/Default")]
[RequireComponent(typeof(PlayerActions))]
public class DefaultState : ActionBase
{
    public float RollingStartSpeed;
    public CapsuleCollider col;
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
        if (player.Grounded)
        {
            actions.DidDash = false;
            if (player.p_input.GetButtonDown("Jump"))
            {
                bool JumpDisabled = player.Crouching && player.rigidBody.velocity.magnitude < RollingStartSpeed;
                if (!JumpDisabled)
                {
                    actions.animator.PlayJumpSound();
                    actions.animator.PlayJumpVoice();
                    if (actions.CheckForState(typeof(JumpState))) actions.ChangeState(typeof(JumpState));
                }
            }
            if (player.p_input.GetAxis("Roll") < 0.7 && player.p_input.GetAxis("Roll") > 0.1)
            {
                player.Crouching = true;
            }
            if (player.p_input.GetAxis("Roll") <= 0.1)
            {
                player.Crouching = false;
            }
            if (speed < RollingStartSpeed)
            {
                if (player.p_input.GetAxis("Roll") >= 0.7)
                {
                    col.height = CrouchHeight;
                    col.center = new Vector3(0, CrouchOffset, 0);
                    actions.ChangeState(typeof(SpinDashState));
                }
            }

            col.height = player.Crouching ? CrouchHeight : 1f;
            Vector3 Offset = new Vector3(0, player.Crouching ? CrouchOffset : 0f, 0);
            col.center = Offset;

            //actions.homingState.OnExit();

        }
        else
        {
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
