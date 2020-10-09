using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepHandler : MonoBehaviour
{
    [System.Serializable] public class Footstep
    {
        public string tag;
        public AudioClip[] clips;
    }
    public List<Footstep> footsteps = new List<Footstep>();
    public AudioSource footSource;
    public Transform FootLeft;
    public Transform FootRight;
    string currentSurface;

    private void Start()
    {
        currentSurface = "rock";
    }

    public void PlayFootstep(string foot)
    {
        string f = foot.ToLower();
        Footstep currentFootstep = new Footstep();
        for (int i = 0; i < footsteps.Count; i++)
        {
            if (footsteps[i].tag == currentSurface)
                currentFootstep = footsteps[i];
        }

        footSource.PlayOneShot(currentFootstep.clips[Random.Range(0, currentFootstep.clips.Length)]);
    }

}
