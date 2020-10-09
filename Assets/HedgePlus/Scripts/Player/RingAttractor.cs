using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingAttractor : MonoBehaviour
{
    PlayerController Player;
    public float AttractionRadius; //Maximum distance for rings to be attracted
    public float AttractionSpeed; //How fast the rings are drawn to Sonic
    public AnimationCurve SpeedOverVelocity; //Adjusts the attraction speed over Sonic's speed
    List<RingActor> closeRings = new List<RingActor>();

    private void Start()
    {
        Player = GetComponent<PlayerController>();
    }
    // Update is called once per frame
    void Update()
    {
        GetClosestObjects();

        float SpeedMod = SpeedOverVelocity.Evaluate(Player.rigidBody.velocity.magnitude / Player.TopSpeed);
        for (int i = 0; i < closeRings.Count; i++)
        {
            if (closeRings[i].gameObject.activeSelf)
            {
                closeRings[i].transform.position = Vector3.MoveTowards(closeRings[i].transform.position, transform.position, AttractionSpeed * SpeedMod);
            } else
            {
                closeRings.RemoveAt(i);
            }
        }
    }

    void GetClosestObjects()
    {
        RingActor[] Rings = FindObjectsOfType<RingActor>();
        RingActor tMin = null;
        float Distance = Mathf.Pow(AttractionRadius, 2);
        foreach (RingActor r in Rings)
        {
            if (!r.gameObject.activeSelf || r.IsLostRing)
                return;
            float currentDistance = (transform.position - r.transform.position).sqrMagnitude;
            if (currentDistance <= Distance)
            {
                tMin = r;
                if (!closeRings.Contains(tMin))
                    closeRings.Add(tMin);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AttractionRadius);
    }
}
