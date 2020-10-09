using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float MaxDistance = 20f;
    public string Pool;
    Transform Player;
    public GameObject spawnedObject { get; set; }
    ObjectPool poolManager;
    public bool DidSpawn { get; set; }

    //Object Placement Settings
    public bool ShowPlacementSettings;
    public float HeightFromGround;
    public bool Align;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        poolManager = ObjectPool.PoolManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance (Player.position, transform.position) < MaxDistance)
        {
            if (!DidSpawn)
            {
                spawnedObject = poolManager.SpawnFromPool(Pool, transform.position, transform.rotation);
                DidSpawn = true;
            }

        } else
        {
            if (spawnedObject != null)
            {
                Despawn();
                DidSpawn = false;
            }
        }

        if (DidSpawn && !spawnedObject.activeSelf)
            gameObject.SetActive(false);
    }

    public void Despawn()
    {
        spawnedObject.transform.position = Vector3.zero;
        spawnedObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        spawnedObject.SetActive(false);
        spawnedObject = null;
    }

    public void AlignToGround()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit))
        {
            transform.position = hit.point + hit.normal * HeightFromGround;
            if (Align) transform.up = hit.normal;
        }
    }
}
