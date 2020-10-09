using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    public float Lifetime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("KillObject");
    }

    IEnumerator KillObject()
    {
        yield return new WaitForSeconds(Lifetime);
        Destroy(gameObject);
    }
}
