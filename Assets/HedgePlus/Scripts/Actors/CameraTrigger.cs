using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class CameraTrigger : MonoBehaviour
{
    public enum CamState { Auto, Free, Fixed }
    public CamState camState;
    public Transform TargetPosition;
    public bool ChangeDirection;
    public bool ResetOnExit;
    Transform DirectionHandle;
    public Vector3 Direction { get; set; }

    private void Update()
    {
        DirectionHandle = transform.Find("Handle");
        if (DirectionHandle != null)
        {
            Direction = Vector3.Normalize(DirectionHandle.position - transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Direction * 3f);
    }
}
