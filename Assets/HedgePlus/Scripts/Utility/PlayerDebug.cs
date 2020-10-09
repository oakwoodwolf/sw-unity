using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerDebug : MonoBehaviour
{
    public PlayerController Player;
    public TextMeshProUGUI DebugText;

    void Update()
    {
        if (!Player || !DebugText) return; //This script will not update if there is no Player or Text assigned.

        //Now that we're sure we have the necessary components, we debug Sonic's current state using its type.
        string StateDebug = "Current State: " + PlayerActions.currentState.GetType().ToString();

        //Next we split Sonic's velocity to planar and vertical so we can debug our ground speed and air speed.
        HedgeMath.SplitPlanarVector(Player.rigidBody.velocity, Player.GroundNormal, out Vector3 GroundSpeed, out Vector3 AirSpeed);
        string GroundDebug = "Ground Speed: " + GroundSpeed.magnitude.ToString("F2") + "m/s";
        string AirDebug = "Air Speed: " + AirSpeed.magnitude.ToString("F2") + "m/s";

        //Set the debug text
        DebugText.text = StateDebug + "\n" + GroundDebug + "\n" + AirDebug;
    }
}
