using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(Spawner))]

public class SpawnerEditor : Editor
{
    Spawner _s;
    private void OnEnable()
    {
        _s = target as Spawner;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //base.OnInspectorGUI();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxDistance"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Pool"));

        _s.ShowPlacementSettings = EditorGUILayout.Foldout(_s.ShowPlacementSettings, "Placement Settings");
        if (_s.ShowPlacementSettings)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("HeightFromGround"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Align"), new GUIContent("Align to Normal"));
            if (GUILayout.Button("Adjust Position")) _s.AlignToGround();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
