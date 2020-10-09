using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
public class MovingPlatform : MonoBehaviour
{
    public Spline path;
    public Rigidbody platform;
    public float speed;
    [SerializeField] [Range(0, 1)] float StartPosition;
    [Range(0, 1)] float Position;
    public Vector3 deltaPosition;
    public AnimationCurve LinearMovementCurve;
    Vector3 PreviousPos;
    Vector3 CurrentPos;
    float Length;
    bool Back; //Controls which direction the platform will move if it is not looping.
    float t;
    private void Start()
    {
        Length = path.Length;
        CurrentPos = platform.position;
        PreviousPos = CurrentPos;
        Position = StartPosition;
        t = StartPosition;
    }
    private void FixedUpdate()
    {

        if (!path.IsLoop)
        {
            /*
            //Add or Subtract the spline position based on what direction we need the platform to move. If the timer is greater than the spline's length
            //or less than 0, we change direction
            if (!Back) Position += Time.fixedDeltaTime * speed;
            else Position -= Time.fixedDeltaTime * speed;
            if (Position >= 0.99f && !Back)
            {
                t = 1;
                Back = true;
            }
            if (Position <= 0.01f && Back) Back = false;
            */

            t = Mathf.PingPong(Time.time * speed, 1f);
            Position = Mathf.Lerp (0, 1, LinearMovementCurve.Evaluate(t));
        } else
        {
            //If the spline path is a loop, we simply set the timer to 0 if it reaches the end.
            Position += Time.fixedDeltaTime * speed;
            if (Position >= 0.99f)
            {
                Position = 0;
            }
        }
        //Get the delta position
        CurrentPos = platform.position;
        if (PreviousPos != CurrentPos)
        {
            deltaPosition = CurrentPos - PreviousPos;
            PreviousPos = CurrentPos;
        }
        //Then we get the position we need on the spline based on the timer, and move the platform to it.
        //Since SplineMesh returns spline positions in local space, we need to use TransformPoint to get it in world space.
        CurveSample p_sample = path.GetSampleAtDistance(Position * Length);
        platform.MovePosition(transform.TransformPoint(p_sample.location));
    }
}
