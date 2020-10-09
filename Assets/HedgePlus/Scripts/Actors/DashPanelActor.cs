using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class DashPanelActor : MonoBehaviour
{
    AudioSource source;
    public float Force;
    public float LockDuration;
    public bool SnapToPosition;
    public bool IsAdditive;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            source = GetComponent<AudioSource>();
            source.Play();
        }
    }
}
