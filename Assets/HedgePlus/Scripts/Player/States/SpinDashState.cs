using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Pinball/Actions/Spin Dash")]
[RequireComponent(typeof(PlayerActions))]
public class SpinDashState : ActionBase
{
    public CapsuleCollider col;
    public enum DashStyle { Classic, Adventure }
    public DashStyle spinDashType;

    public int MaxRevs;
    public float TurnRate = 15f;
    public float MaxCharge;
    public float ChargeDuration; //How long does it take to reach max charge
    public float SpinDashCharge { get; set; }
    float RevAmount;

    public bool ChargeWhileRunning;
    public override void InitializeState(PlayerController p, PlayerActions a)
    {
        base.InitializeState(p, a);
        
        Debug.Log("Current State: Spin Dash");
        if (player != null)
            Debug.Log("Found Player Controller");
        if (actions != null)
            Debug.Log("Found Action Controller");
        //col.height = actions.defaultState.CrouchHeight;
        //col.center = new Vector3(0, actions.defaultState.CrouchOffset, 0);
        actions.StateIndex = 4;
        SpinDashCharge = 15;
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
        if (player.InputDir.sqrMagnitude > 0.01f)
        {
            Quaternion b = Quaternion.LookRotation(Vector3.ProjectOnPlane(player.InputDir, player.GroundNormal), player.GroundNormal);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, b, Time.deltaTime * TurnRate);
        }
        if (SpinDashCharge < MaxCharge)
        {
            switch (spinDashType)
            {
                case DashStyle.Adventure:
                    player.p_input.SetVibration(0, ((SpinDashCharge / MaxCharge) * 0.35f), 0.25f);
                    SpinDashCharge += MaxCharge * (Time.deltaTime / ChargeDuration);
                    break;
                case DashStyle.Classic:
                    if (player.p_input.GetButtonDown("Jump"))
                    {
                        SpinDashCharge += RevAmount;
                        actions.animator.SpinDashSingle();
                    }
                    break;
            }
        }
        if (player.p_input.GetAxis("Roll") < 0.1 && !player.p_input.GetButton("SpinDash"))
        {
            actions.animator.SpinDashRelease();
            player.Crouching = true;
            player.rigidBody.velocity += player.transform.forward * SpinDashCharge;
            actions.ChangeState(typeof(DefaultState));
        }
        if (player.p_input.GetButtonDown("Jump") && spinDashType == DashStyle.Adventure)
        {

            actions.animator.PlayJumpSound();
            actions.animator.PlayJumpVoice();
            if (actions.CheckForState(typeof(JumpState))) actions.ChangeState(typeof(JumpState));

        }
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        player.rigidBody.velocity = new Vector3(player.rigidBody.velocity.x /1.1f, player.rigidBody.velocity.y, player.rigidBody.velocity.z /1.1f);
       // player.rigidBody.velocity = Vector3.zero;
    }
}
