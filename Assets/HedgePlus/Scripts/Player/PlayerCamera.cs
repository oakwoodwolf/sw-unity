using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public enum CameraState { Auto, Free, Fixed }
    public CameraState camState;
    [SerializeField] Transform FollowTarget;
    [SerializeField] PlayerController Player;
    [SerializeField] float MaxDistance = 4f;
    [SerializeField] float MaxHeight = 1f;
    [SerializeField] float YOffset = 0.1f;
    [SerializeField] float PositionDamping = 0.1f;
    [SerializeField] float TransitionSpeed = 5f;
    [SerializeField] float RotationSpeed = 15f;
    [SerializeField] float CollisionRadius = 0.1f;


    Vector3 LookDir;
    Vector3 FollowDir;
    Vector3 TargetOffset;
    Quaternion LookRot;
    public Vector3 TargetPosition { get; set; }
    public bool UpdateCameraOrientation = true;
    bool SmoothPosition;

    Vector3 PositionDamp;
    // Start is called before the first frame update
    void Start()
    {
        //Upon loading, we will be setting the camera to its most default position and direction.
        TargetOffset = FollowTarget.position + FollowTarget.up * YOffset;
        LookDir = TargetOffset - transform.position;
        LookDir.Normalize();
        SmoothPosition = true;
    }

    // Update is called once per frame
    void Update()
    {
        ///In Update we will be adding an offset to the player so they are not dead center.
        ///Then while we want to update the orientation (i.e. not in a camera trigger) we will simply be setting the camera's look
        ///direction to the direction from the camera to Sonic, and normalizing it. Then we smooth out the direction by
        ///simply lerping it to Look Dir projected onto Sonic's up vector.
        TargetOffset = FollowTarget.position + FollowTarget.up * YOffset;
        if (UpdateCameraOrientation)
        {
            LookDir = TargetOffset - transform.position;
            LookDir.Normalize();
        }
        FollowDir = Vector3.Slerp(FollowDir, Vector3.ProjectOnPlane(LookDir, FollowTarget.up), Time.deltaTime * RotationSpeed);
    }

    private void LateUpdate()
    {
        switch (camState)
        {
            case CameraState.Auto:
                ///For Auto Camera, we are simply using the camera's FollowDir to position it, then using SmoothDamp to move it into position smoothly.
                ///Then we simply make it look at Sonic.
                if (SmoothPosition)
                {
                    TargetPosition = TargetOffset + FollowTarget.up * MaxHeight + -FollowDir * MaxDistance;
                    transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref PositionDamp, PositionDamping);
                } else
                {
                    transform.position = TargetPosition;
                }
                LookRot = Quaternion.LookRotation(TargetOffset - transform.position, FollowTarget.up);
                break;
            case CameraState.Fixed:
                ///For Fixed Camera, we just lerp the camera into the position we want and make it look at Sonic.
                transform.position = Vector3.Lerp(transform.position, TargetPosition, TransitionSpeed * Time.deltaTime);
                LookRot = Quaternion.LookRotation(TargetOffset - transform.position, Vector3.up);
                break;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, LookRot, Time.deltaTime * RotationSpeed);

    }

    public void RespawnCamera (Vector3 Position, Vector3 Forward)
    {
        ///What this is SUPPOSED to do is revert the camera to the most default position we can achieve.
        //camState = CameraState.Auto;
        SmoothPosition = false;
        UpdateCameraOrientation = true;
        TargetPosition = Position + FollowTarget.up * MaxHeight - FollowTarget.forward * MaxDistance;
        camState = CameraState.Auto;
        StartCoroutine("HardResetCamera");
    }

    public void SetLookDir (Vector3 Direction)
    {
        LookDir = Direction;
        Debug.Log("Set camera direction to " + Direction);
    }

    IEnumerator HardResetCamera()
    {
        ///This disables smooth camera movement for a single frame to hard reset it.
        transform.position = TargetPosition;
        yield return null;
        SmoothPosition = true;
    }




}
