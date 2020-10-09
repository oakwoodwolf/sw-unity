using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepOffsetter : MonoBehaviour
{
    PlayerController Player;
    public float MaxStepHeight = 0.1f; //Highest step Sonic can adjust to
    public float MinStepHeight = 0.01f; //Lowest step Sonic can adjust to
    public float SpeedThreshold = 10f; //If Sonic is moving past this speed, we will use his velocity for checking. If not, we will use input.
    // Start is called before the first frame update
    void Start()
    {
        Player = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (Player.Grounded)
        {
            AdjustToSteps();
        }
    }

    void AdjustToSteps()
    {
        
        
    }
}
