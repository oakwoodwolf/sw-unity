using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalRingActor : MonoBehaviour
{
    public bool UseGlobalRotation;
    public float LocalRotationSpeed;
    public Transform GoalRingMesh;
    Vector3 Eulers;

    void Update()
    {
        if (UseGlobalRotation)
        {
            GoalRingMesh.localRotation = Quaternion.Euler(RingManager.RingEulers);
        } else
        {
            Eulers.y += LocalRotationSpeed * Time.deltaTime;
            GoalRingMesh.localRotation = Quaternion.Euler(Eulers);
        }
    }
}
