using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEditor;
using UnityEngine;

[AddComponentMenu("Pinball/Actions/Fly")]
[RequireComponent(typeof(PlayerActions))]
public class FlyState : ActionBase
{
    public GameObject particle;
    public float flightTimer = 3f;
    private float timer;
    public float flightAccel = 17f;
    public float flightGravity = 0.25f;
    private float oldGravity = -36;
    private Vector3 AirVelocity;
    public TextMeshProUGUI timerDisplay;
    private bool tiredPlayed = false;
    public GameObject timerHUD;
    GameObject FlyParticleClone;
    public override void InitializeState(PlayerController p, PlayerActions a)
    {
        base.InitializeState(p, a);
        Debug.Log("Current State: Flight");
        timerHUD.SetActive(true);
        timer = flightTimer;
        player.TopSpeed /= 2;
        oldGravity = player.Gravity.y;
        actions.StateIndex = 12;
        tiredPlayed = false;
        player.Grounded = false;
        actions.animator.PlayHomingVoice();

        player.rigidBody.velocity = Vector3.ProjectOnPlane(player.rigidBody.velocity, player.GroundNormal);
        AirVelocity = player.rigidBody.velocity;
        player.rigidBody.velocity = AirVelocity;
        FlyParticleClone = Instantiate(particle, player.transform.position, Quaternion.identity);
        FlyParticleClone.transform.parent = player.transform;

        //player.rigidBody.position += player.GroundNormal * player.GroundCheckDistance;
    }

    public override void UpdateState()
    {
        actions.UpdateTargets();
        timerDisplay.text = ((int)timer).ToString();


    }
    public override void FixedUpdateState()
    {   
        Vector3 vector = new Vector3(AirVelocity.x, 0f, AirVelocity.z);
        HedgeMath.SplitPlanarVector(player.rigidBody.velocity, player.GroundNormal, out Vector3 GroundSpeed, out Vector3 AirSpeed);
        //player.TurnRate = Mathf.Lerp(player.TurnRate, player.TurnSpeed, Time.fixedDeltaTime * player.TurnSmoothing);
        //float Handling = player.TurnRate * player.AirHandlingAmount;
        player.Gravity.y = oldGravity * flightGravity;
        if (player.rigidBody.velocity.magnitude != 0f)
        {
            vector = player.transform.forward * AirSpeed.magnitude;
        }
        player.transform.rotation = Quaternion.LookRotation(player.transform.forward, player.transform.up);
        if (player.p_input.GetButton("Jump"))
        {
            if (timer > 0f) {
                

                    player.rigidBody.AddForce(player.GroundNormal * flightAccel, ForceMode.Impulse);
                timer -= Time.fixedDeltaTime;
            }
           
        }
        if (timer <= 0f && !tiredPlayed) {
            actions.animator.PlayHurtVoice();
            actions.StateIndex = 13;
            tiredPlayed = true;
        } 

        if (player.Grounded || player.p_input.GetButtonDown("Roll"))
        {
            exitFlight();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<RingActor>(out RingActor ring))
        {
            return;
        }
        if (!other.TryGetComponent<DashRingActor>(out DashRingActor dashRing))
        {
            exitFlight();
        }
        else timer = flightTimer;
        
    }

    private void exitFlight()
    {
        Destroy(FlyParticleClone);
        timer = flightTimer;
        player.Gravity.y = oldGravity;
        player.TopSpeed = 60f;
        timerHUD.SetActive(false);
        actions.ChangeState(typeof(DefaultState));
    }
    }
