using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StageSetup : EditorWindow
{
    ObjectPool _pool;
    [MenuItem("Pinball SDK/Object Placer")]
    static void Init()
    {
        StageSetup window = (StageSetup)EditorWindow.GetWindow(typeof(StageSetup), false, "Object Placer", true);
        window.Show();
    }

    private void OnGUI()
    {
        _pool = FindObjectOfType<ObjectPool>();
        if (_pool == null)
        {
            GUILayout.Label("Add an object pool to get started.", EditorStyles.helpBox);
            if (GUILayout.Button("Add Object Pool"))
            {
                GameObject newPool = NewObjectPool();
                Selection.activeGameObject = newPool;
                Undo.RecordObject(newPool, "Object Pool");
            }
        } else
        {
            GUILayout.Label("Pooled Objects", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Ring"))
            {
                Object ring = AssetDatabase.LoadAssetAtPath("Assets/HedgePlus/Prefabs/Spawners/Objects/RingSpawner.prefab", typeof(Object));
                Selection.activeObject = PrefabUtility.InstantiatePrefab(ring as GameObject);
                MoveToView();
                Undo.RecordObject(Selection.activeObject, "Ring");
            }
        }
        GUILayout.Label("Stage Objects", EditorStyles.boldLabel);
        if (GUILayout.Button("Add Item Capsule"))
        {
            Object cap = AssetDatabase.LoadAssetAtPath("Assets/HedgePlus/Prefabs/Objects/ItemCap.prefab", typeof(Object));
            Selection.activeObject = PrefabUtility.InstantiatePrefab(cap as GameObject);
            MoveToView();
            Undo.RecordObject(Selection.activeObject, "Item Capsule");
        }
        if (GUILayout.Button("Add Spring"))
        {
            Object spring = AssetDatabase.LoadAssetAtPath("Assets/HedgePlus/Prefabs/Objects/SpringObject.prefab", typeof(Object));
            Selection.activeObject = PrefabUtility.InstantiatePrefab(spring as GameObject);
            MoveToView();
            Undo.RecordObject(Selection.activeObject, "Spring");
        }
        if (GUILayout.Button("Add Dash Panel"))
        {
            Object speedpad = AssetDatabase.LoadAssetAtPath("Assets/HedgePlus/Prefabs/Objects/DashPanelObject.prefab", typeof(Object));
            Selection.activeObject = PrefabUtility.InstantiatePrefab(speedpad as GameObject);
            MoveToView();
            Undo.RecordObject(Selection.activeObject, "Dash Panel");
        }

        GUILayout.Label("Utility", EditorStyles.boldLabel);
        if (GUILayout.Button("Add Spline"))
        {
            GameObject spline = NewSpline();
            Selection.activeGameObject = spline;
            MoveToView();
            Undo.RecordObject(Selection.activeObject, "Spline");
        }
    }


    GameObject NewObjectPool()
    {
        GameObject pool = new GameObject();
        pool.AddComponent<ObjectPool>();
        pool.name = "Object Pool";
        ObjectPool.Pool ringPool = new ObjectPool.Pool();
        ringPool.Name = "Ring";
        ringPool.Amount = 100;
        ringPool.Prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/HedgePlus/Prefabs/Objects/RingObject.prefab", typeof(GameObject));
        GameObject RingParent = new GameObject();
        RingParent.name = "Rings";
        RingParent.transform.parent = pool.transform;
        ringPool.Container = RingParent.transform;
        pool.GetComponent<ObjectPool>().ObjectPools.Add(ringPool);
        return pool;
    }

    GameObject NewSpline()
    {
        GameObject splineHolder = new GameObject();
        splineHolder.name = "Spline Object";
        GameObject Spline = new GameObject();
        Spline.AddComponent<SplineMesh.Spline>();
        Spline.name = "Spline";
        Spline.transform.parent = splineHolder.transform;
        return splineHolder;
    }

    static void MoveToView()
    {
        SceneView.FocusWindowIfItsOpen<UnityEditor.SceneView>();
        if (SceneView.lastActiveSceneView == null)
            return;
        Camera cam = SceneView.lastActiveSceneView.camera;
        GameObject tgt = Selection.activeGameObject;
        if (cam == null || tgt == null) return;

        tgt.transform.position = cam.transform.position + cam.transform.forward * 15f;
    }
}
