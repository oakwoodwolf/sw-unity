using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SpringActor))]
public class SpringEditor : Editor
{
    SpringActor spring;
    private void OnEnable()
    {
        spring = target as SpringActor;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (spring.Target != null)
        {
            if (GUILayout.Button("Target Object"))
            {
                spring.TargetObject();
            }
        }
    }
}
