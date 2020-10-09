using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : Editor
{
    MovingPlatform tgt;
    public void OnEnable()
    {
        tgt = target as MovingPlatform;
    }

    public override void OnInspectorGUI()
    {
        if (tgt.path == null)
        {
            EditorGUILayout.HelpBox("No spline has been assigned. Please add a spline.", MessageType.Error);
        }
        if (tgt.platform == null)
        {
            EditorGUILayout.HelpBox("No rigidbody has been assigned. Please add a rigidbody.", MessageType.Error);
        }
        base.OnInspectorGUI();
    }
}
