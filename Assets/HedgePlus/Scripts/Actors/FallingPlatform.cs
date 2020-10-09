using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    Rigidbody rigidBody;
    public enum PlatformState { Idle, Shake, Drop }
    [HideInInspector] public PlatformState state;
    public Transform Mesh;
    public float Lifetime; //How long can Sonic stand on the platform before it drops
    public float ShakeTime; //How long does the platform shake to indicate that it is going to fall
    public Vector3 Gravity;

    public float ShakeSpeed;
    public float ShakeIntensity;
    Vector3 InitialPos;
    Vector3 StartPosition;
    Vector3 Velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        InitialPos = Mesh.localPosition;
        StartPosition = transform.position;
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == PlatformState.Shake)
        {
            ShakeMesh();
        } else
        {
            Mesh.localPosition = InitialPos;
        }

        if (state == PlatformState.Idle)
        {
            transform.position = StartPosition;
            Velocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (state == PlatformState.Drop)
        {
            Velocity += Gravity * Time.fixedDeltaTime;
            rigidBody.MovePosition(rigidBody.position + Velocity * Time.fixedDeltaTime);
        }
    }

    void ShakeMesh()
    {
        Vector3 Pos = InitialPos;
        Pos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * ShakeIntensity) * ShakeSpeed;
        Mesh.localPosition = Pos;
    }

    IEnumerator PlatformBehavior()
    {
        float TimeUntilShake = Lifetime - ShakeTime;
        yield return new WaitForSeconds(TimeUntilShake);
        state = PlatformState.Shake;
        yield return new WaitForSeconds(ShakeTime);
        state = PlatformState.Drop;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine("PlatformBehavior");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("KillPlane"))
        {
            gameObject.SetActive(false);
        }
    }
}
