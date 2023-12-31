﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class PulleyActor : MonoBehaviour
{
    public GameObject HomingTarget;
    public Transform Handle;
    public Transform HandleGripPos;
    public float MaxLength;
    [Range(0, 1)] public float PulleyPosition = 1;
    public float Speed;
    public bool DeployedOnStart;
    public bool Moving { get; protected set; }
    [Header("Visuals and effects")]
    public LineRenderer wire;
    public Transform WireTopAnchor;
    public Transform WireBottomAnchor;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying && DeployedOnStart)
        {
            DeployPulley();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Handle) return;
        Vector3 Pos = Handle.localPosition;
        Pos.y = -MaxLength * PulleyPosition;
        Handle.localPosition = Pos;

        if (!wire || !WireTopAnchor || !WireBottomAnchor) return; //This won't update this bit if any of these are unassigned
        wire.SetPosition(0, WireTopAnchor.position);
        wire.SetPosition(1, WireBottomAnchor.position);

        if (Application.isPlaying)
        {
            HomingTarget.SetActive(!Moving);
        }
    }

    public void DeployPulley()
    {
        Moving = true;
        audioSource.Play();
        StartCoroutine(SetPulleyPosition(1f));
    }

    public void RetractPulley()
    {
        Moving = true;
        audioSource.Play();
        StartCoroutine(SetPulleyPosition(0f));
    }

    IEnumerator SetPulleyPosition (float Target)
    {
        //Check if position is greater or less than target
        if (Target > PulleyPosition)
        {
            while (PulleyPosition < Target)
            {
                PulleyPosition += Time.fixedDeltaTime * Speed;
                yield return null;
                if (PulleyPosition >= Target)
                {
                    PulleyPosition = Target;
                    audioSource.Stop();
                    Moving = false;
                    break;
                }
            }
        } else if (Target < PulleyPosition)
        {
            while (PulleyPosition > Target)
            {
                PulleyPosition -= Time.fixedDeltaTime * Speed;
                yield return null;
                if (PulleyPosition <= Target)
                {
                    PulleyPosition = Target;
                    audioSource.Stop();
                    Moving = false;
                    StartCoroutine(ResetPulley(2f));
                    break;
                }
            }
        }
        yield return null;
    }

    IEnumerator ResetPulley (float delay)
    {
        yield return new WaitForSeconds(delay);
        DeployPulley();
    }
}
