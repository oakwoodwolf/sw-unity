using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingActor : MonoBehaviour
{
    [SerializeField] Transform Mesh;
    public bool IsLostRing;
    public float Lifetime = 5f;
    float t;
    public float FlickerRate = 5f; //This is how many frames the mesh will be active and inactive for.
    public AnimationCurve FlickerOverLifetime; //This exists solely to make the mesh flicker faster as it gets closer to despawning.
    public float FlickerStartTime;
    float frameTimer;
    int Framerate = 60;
    private void OnEnable()
    {
        Mesh.gameObject.SetActive(true); //Enabling the mesh, on the chance that it was disabled upon despawning
        if (IsLostRing)
        {
            t = Lifetime;
            frameTimer = FlickerRate;
        }
    }

    private void Update()
    {
        //Rotate the ring
        Mesh.localRotation = Quaternion.Euler(RingManager.RingEulers);
        if (IsLostRing)
        {
            //We have two timers going for lost rings. The first timer is the actual lifetime of the ring, and the second controls the flickering
            //that lets players know that the ring is going to despawn soon. The lifetime is delta time based while the flickering is frame-based.
            t -= Time.deltaTime;
            if (t < FlickerStartTime)
            {
                frameTimer -=Time.deltaTime * Framerate;
            }
            if (t <= 0)
            {
                gameObject.SetActive(false);
            }
            if (frameTimer <= 0)
            {
                Mesh.gameObject.SetActive(!Mesh.gameObject.activeSelf);
                frameTimer = FlickerRate * FlickerOverLifetime.Evaluate(t / (Lifetime - FlickerStartTime));
            }
        }
    }
}
