using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingManager : MonoBehaviour
{
    public static Vector3 RingEulers;
    public float RotationSpeed;

    void Update()
    {
        RingEulers.y += RotationSpeed * Time.deltaTime;
    }
}
