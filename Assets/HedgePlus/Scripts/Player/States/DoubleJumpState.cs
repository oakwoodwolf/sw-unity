using UnityEngine;
using Rewired;
[AddComponentMenu("Pinball/Actions/Double Jump")]
[RequireComponent(typeof(PlayerActions))]
public class DoubleJumpState : ActionBase
{
    public float Force = 0.75f;
    public override void InitializeState(PlayerController p, PlayerActions a)
    {
        base.InitializeState(p, a);
        actions.StateIndex = 1;
        player.rigidBody.velocity = Vector3.ProjectOnPlane(player.rigidBody.velocity, player.GroundNormal);
        player.rigidBody.AddForce(player.GroundNormal * Force, ForceMode.Impulse);
        actions.animator.PlayDoubleJumpSound();
    }

    public override void UpdateState()
    {
        if (player.p_input.GetButtonDown("Jump"))
        {
            if (actions.ClosestTarget != null)
            {
                if (actions.CheckForState(typeof(HomingState)))
                {
                    actions.ChangeState(typeof(HomingState));
                }

            }
        }

        if (player.Grounded)
        {
            actions.ChangeState(typeof(DefaultState));
        }
    }
}
