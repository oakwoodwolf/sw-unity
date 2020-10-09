using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class RandomOneOff : MonoBehaviour
{
    AudioSource source;
    public AudioClip[] clips;

    private void OnEnable()
    {
        source = GetComponent<AudioSource>();
        source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}
