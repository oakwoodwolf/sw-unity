using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityDebug : MonoBehaviour
{
    public float Increment = 30f;
    public int Step;
    public Rigidbody rigidBody;
    List<Vector3> Position = new List<Vector3>();
    List<Vector3> Rotation = new List<Vector3>();
    float t;
    // Start is called before the first frame update
    void Start()
    {
        float t = Increment;
    }

    // Update is called once per frame
    void Update()
    {
        t--;
        if (t <= 0)
        {
            Position.Add(transform.position);
            Rotation.Add(-transform.up);
            if (Position.Count > Step)
            {
                Position.RemoveAt(0);
                Rotation.RemoveAt(0);
            }
            t = Increment;
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Position.Count; i++)
        {
            Debug.DrawRay(Position[i], Rotation[i] * 0.5f, Color.magenta);
            if (i + 1 < Position.Count)
                Debug.DrawLine(Position[i], Position[i + 1], Color.magenta);
        }
    }
}
