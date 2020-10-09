using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class SplineController : MonoBehaviour
{
    PlayerController Player;
    public Spline activeSpline;
    // Start is called before the first frame update
    void Start()
    {
        Player = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (activeSpline != null)
        {
            //Get Sonic's current position along the spline
            CurveSample cur = activeSpline.GetSampleAtDistance(GetClosestPos(Player.rigidBody.position));

            //Get the Right vector of the current spline position so we can accurately adjust Sonic's velocity
            Vector3 SplinePlane = Vector3.Cross(cur.tangent, cur.up);

            //Project the vector onto the plane
            Player.rigidBody.velocity = Vector3.ProjectOnPlane(Player.rigidBody.velocity, SplinePlane);

            //Project the input too
            Player.InputDir = Vector3.ProjectOnPlane(Player.InputDir, SplinePlane);

            //Set the Player's position along the spline plane
            Vector3 NewPos = activeSpline.transform.TransformPoint(cur.location);
            NewPos.y = Player.rigidBody.position.y;
            Debug.DrawLine(transform.position, NewPos);
            Player.rigidBody.position = Vector3.MoveTowards(Player.rigidBody.position, NewPos, 1f);
        }
    }

    /// <summary>
    /// Returns the Spline Position closest to the given Transform's position
    /// </summary>
    public float GetClosestPos(Vector3 ColPos)
    {
        float ClosestSample = 0;
        float CurrentDist = 9999999f;
        for (float n = 0; n < activeSpline.Length; n += Time.deltaTime * 10f)
        {
            float dist = ((activeSpline.GetSampleAtDistance(n).location + activeSpline.transform.position) - ColPos).sqrMagnitude;
            if (dist < CurrentDist)
            {
                CurrentDist = dist;
                ClosestSample = n;
            }

        }
        return ClosestSample;
    }
}
