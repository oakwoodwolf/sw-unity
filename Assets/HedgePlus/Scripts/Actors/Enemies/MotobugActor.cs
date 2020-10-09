using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotobugActor : MonoBehaviour
{
    public enum MotoState { Default, Notice, Windup, Charge }
    MotoState currentState;
    Rigidbody rigidBody;
    public Animator animator;
    public SphereCollider col;
    public float PatrolDelay; //How long the motobug waits before continuing
    public float PatrolRadius;
    public float GroundCheckOvershoot;
    public LayerMask GroundLayer;
    Vector3 Destination;
    Vector3 Normal;
    Vector3 Velocity;
    Vector3 Home;
    Vector3 GroundPoint;
    Vector3 Forward;
    bool InTransit;
    float DistanceToTarget;
    float _maxDistance;
    float t;
    [Header("Movement")]
    public float PatrolSpeed;
    public float ChargeSpeed;
    public float WindupDuration;
    public AnimationCurve SpeedOverDistance;

    public float RotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        Home = transform.position;
        Destination = PatrolPoint(Home, PatrolRadius);

        DistanceToTarget = (Destination - transform.position).sqrMagnitude;
        _maxDistance = DistanceToTarget;
        InTransit = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case MotoState.Default:
                animator.SetFloat("Speed", Velocity.magnitude);
                if (!InTransit)
                {
                    t += Time.deltaTime;
                    if (t >= PatrolDelay)
                    {
                        Destination = PatrolPoint(Home, PatrolRadius);
                        _maxDistance = (Destination - transform.position).sqrMagnitude;
                        t = 0;
                        InTransit = true;
                    }
                }
                break;
            default:
                currentState = MotoState.Default;
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case MotoState.Default:
                if (InTransit)
                {
                    DistanceToTarget = (Destination - transform.position).sqrMagnitude;
                    Velocity = (Destination - transform.position).normalized * PatrolSpeed * SpeedOverDistance.Evaluate(DistanceToTarget / _maxDistance);
                    if ((Destination - transform.position).sqrMagnitude <= 0.01f)
                    {
                        Velocity = Vector3.zero;
                        InTransit = false;
                    }

                    //Set rotation
                    if (Velocity.sqrMagnitude > 0.1f)
                    {
                        Vector3 LookDir = Vector3.ProjectOnPlane(Velocity, transform.up);
                        Forward = Vector3.Slerp(Forward, LookDir, Time.fixedDeltaTime * RotationSpeed);
                        Quaternion Rotation = Quaternion.LookRotation(Forward);
                        rigidBody.MoveRotation(Rotation);
                    }
                }
                break;
            default:
                currentState = MotoState.Default;
                break;
        }

        float Height = col.radius / 2;
        if (Physics.Raycast(rigidBody.position, -transform.up, out RaycastHit ground, Height + GroundCheckOvershoot, GroundLayer))
        {
            Normal = ground.normal;
            GroundPoint = ground.point + ground.normal * Height;
        }
        rigidBody.MovePosition(rigidBody.position + Velocity * Time.fixedDeltaTime);
    }

    /// <summary>
    /// This will return a random patrol point within a specified radius of an origin point.
    /// </summary>
    /// <param name="Origin">Center point to generate points from</param>
    /// <param name="Radius">How far from the origin we can go</param>
    /// <returns></returns>
    Vector3 PatrolPoint(Vector3 Origin, float Radius)
    {
        Vector3 point = new Vector3();
        Vector3 Rand = Random.insideUnitSphere * Radius;
        Rand += Origin;
        Rand.y = transform.position.y;

        //First we check that the point is not obscured by any walls
        if (!Physics.Linecast (transform.position, Rand))
        {
            //Next we raycast down from the generated point to make sure it is not hanging off of a ledge or anything
            if (Physics.Raycast (Rand, -Vector3.up, out RaycastHit hit, 1f))
            {
                point = Rand;
            } else
            {
                //No ground under the generated point, so we return to the origin.
                point = Origin;
            }
        } 
        else
        {
            //If it is obscured by a wall, we simply return to the origin point.
            point = Origin;
        }

        return point;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.DrawSphere(Destination, 0.1f);
            Gizmos.DrawWireSphere(Home, PatrolRadius);
        } else
        {
            Gizmos.DrawWireSphere(transform.position, PatrolRadius);
        }
    }


}
