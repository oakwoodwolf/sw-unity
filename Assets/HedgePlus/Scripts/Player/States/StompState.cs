using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerAnimator;

[AddComponentMenu("Pinball/Actions/Stomp")]
[RequireComponent(typeof(PlayerActions))]
public class StompState : ActionBase
{
    [Header("Stomp")]
    public float Force = 0.75f;
    public GameObject particle;
    [Header("Bounce")]
    public bool canBounce;
    private bool HasBounced;
    private float OriginalBounceFactor;
    private float CurrentBounceAmount;
    public int BounceCount = 0;
    private float BounceBuffer = 0.2f;

    [SerializeField] private List<float> BounceUpSpeeds;
    [SerializeField] float BounceUpMaxSpeed;
    [SerializeField] float BounceConsecutiveFactor;
    [SerializeField] float BounceHaltFactor;
    public float InitialVelocityMultiplier = 0.2f;

    public override void InitializeState(PlayerController p, PlayerActions a)
    {
        base.InitializeState(p, a);
        StartStomp();
    }

    private void StartStomp()
    {
        actions.StateIndex = 9;
        player.rigidBody.velocity = Vector3.ProjectOnPlane(player.rigidBody.velocity, player.GroundNormal);
        actions.animator.SpeedParticles.Play();
        actions.animator.PlayStompSound();
        actions.animator.HomingTrail.emit = true;
        Vector3 velocity = player.rigidBody.velocity;
        velocity.x *= InitialVelocityMultiplier;
        velocity.z *= InitialVelocityMultiplier;
        player.rigidBody.velocity = velocity;
    }

    public override void FixedUpdateState()
    {
        if (player.rigidBody.velocity.y < 0)
        {
            player.rigidBody.velocity += player.GroundNormal * (0f - Force * 30) * Time.fixedDeltaTime;

        }
        if (player.Grounded)
        {
            if (canBounce && player.p_input.GetAxis("Roll") > 0.55 && !HasBounced)
            {
                actions.animator.PlayBounceSound();
                    Bounce(player.GroundNormal);
                
                HasBounced = true;
                
            }
            else
            {
                BounceCount = 0;
                actions.animator.PlayStompImpactSound();
                actions.animator.HomingTrail.emit = false;
                actions.animator.SpeedParticles.Stop();
                actions.ChangeState(typeof(DefaultState));
            }
            
            GameObject part = Instantiate(particle, player.transform.position, Quaternion.identity);
            part.transform.parent = player.transform;
            

        }
        if (HasBounced)
        {
            BounceBuffer -= Time.fixedDeltaTime;
            if (BounceBuffer <= 0) HasBounced = false;
        }
    }

   private void Bounce(Vector3 normal)
    {
        CurrentBounceAmount = BounceUpSpeeds[BounceCount];
        actions.StateIndex = 1;
        CurrentBounceAmount = Mathf.Clamp(CurrentBounceAmount, BounceUpSpeeds[BounceCount], BounceUpMaxSpeed);
        player.rigidBody.position += player.GroundNormal * player.GroundCheckDistance * 2f;
        Vector3 velocity = player.rigidBody.velocity;
        velocity.y = 0f;
        Vector3 vector = Vector3.Lerp(player.GroundNormal, player.InputDir, 0.25f).normalized * (20 * CurrentBounceAmount);
        velocity += vector;
        player.rigidBody.velocity = velocity;
        actions.animator.PlayBounceSound();
        if (BounceCount < BounceUpSpeeds.Count - 1)
        {
            BounceCount++;
        }
    }
    public override void UpdateState()
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
                    else
                    {
                        if (actions.CheckForState(typeof(FlyState)) && !actions.DidDash)
                        {
                            actions.DidDash = true;
                            actions.ChangeState(typeof(FlyState));
                        }
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
            actions.animator.HomingTrail.emit = false;
        }
        if (player.p_input.GetButtonDown("Roll"))
        {
            StartStomp();
        }
    }
}
