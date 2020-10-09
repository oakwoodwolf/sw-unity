using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class SpringActor : MonoBehaviour
{
    AudioSource source;
    public float SpringForce = 15f;
    public float PositionOffset = 1f;
    public float LockDuration = 1f;

    public ParticleSystem springParticles;

    public Transform Target;

    public bool DebugForce;
    public bool DebugOffset;
    Vector3 Gravity = new Vector3 (0, -32f, 0);
    float Drag = 0.065f;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        source.Play();
        springParticles.Play();
        GetComponent<Animator>().SetTrigger("Hit");
    }

    private void Update()
    {
        if (DebugOffset)
            Debug.DrawLine(transform.position, transform.position + transform.up * PositionOffset);
        if (DebugForce)
        {
            if (LockDuration > 0)
            {
                Vector3[] DebugTrajectoryPoints = PreviewTrajectory(transform.position, transform.up * SpringForce, Gravity, Drag, LockDuration);
                for (int i = 1; i < DebugTrajectoryPoints.Length; i++)
                {
                    Debug.DrawLine(DebugTrajectoryPoints[i - 1], DebugTrajectoryPoints[i]);
                }
            }
        }
    }

    static Vector3[] PreviewTrajectory(Vector3 position, Vector3 velocity, Vector3 gravity, float drag, float time)
    {
        float timeStep = Time.fixedDeltaTime;
        int iterations = Mathf.CeilToInt(time / timeStep);
        if (iterations < 2)
        {
            Debug.LogError("PreviewTrajectory (Vector3, Vector3, Vector3, float, float): Unable to preview trajectory shorter than Time.fixedDeltaTime * 2");
            return new Vector3[0];
        }
        Vector3[] path = new Vector3[iterations];
        Vector3 pos = position;
        Vector3 vel = velocity;
        path[0] = pos;
        float dragScale = Mathf.Clamp01(1.0f - (drag * timeStep));
        for (int i = 1; i < iterations; i++)
        {
            vel = vel + (gravity * timeStep);
            vel *= dragScale;
            pos = pos + (vel * timeStep);
            path[i] = pos;
        }
        return path;
    }

    public void TargetObject ()
    {
        if (Target != null)
        {
            Vector3 Direction = Target.position - transform.position;
            transform.up = Direction.normalized;
        } else
        {
            Debug.LogError("No target was set.");
        }
    }
}
