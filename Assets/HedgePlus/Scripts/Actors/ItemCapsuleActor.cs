using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class ItemCapsuleActor : MonoBehaviour
{
    public bool IsActive;
    public bool CanRespawn { get; set; } //If true, capsule will respawn with Sonic and other objects. If false, it will remain inactive after dying or respawning.
    public enum Item
    {
        TenRings, FiveRings, OneUp, Shield, Invincible
    }
    public Item capItem;

    [Header("Meshes and materials")]
    public GameObject[] DisableWhenHit;
    public Renderer ItemRenderer;
    public Material TenRing, FiveRing, OneUp, Shield, Invincible;

    public float BobAmplitude = 0.1f;
    public float BobSpeed = 1f;
    public float yOffset;
    Vector3 InitialPos;

    private void Start()
    {
        InitialPos = Vector3.zero;
        if (capItem == Item.OneUp || capItem == Item.Invincible)
        {
            CanRespawn = false;
        } else
        {
            CanRespawn = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (ItemRenderer != null)
        {
            //Here we change the item renderer's material based on which item this capsule gives
            switch (capItem)
            {
                case Item.TenRings:
                    if (TenRing == null)
                    {
                        Debug.Log("Material for item " + capItem.ToString() + " has not been assigned.");
                        return;
                    }
                    ItemRenderer.material = TenRing;
                    break;
                case Item.FiveRings:
                    if (FiveRing == null)
                    {
                        Debug.Log("Material for item " + capItem.ToString() + " has not been assigned.");
                        return;
                    }
                    ItemRenderer.material = FiveRing;
                    break;
                case Item.OneUp:
                    if (OneUp == null)
                    {
                        Debug.Log("Material for item " + capItem.ToString() + " has not been assigned.");
                        return;
                    }
                    ItemRenderer.material = OneUp;
                    break;
                case Item.Shield:
                    if (Shield == null)
                    {
                        Debug.Log("Material for item " + capItem.ToString() + " has not been assigned.");
                        return;
                    }
                    ItemRenderer.material = Shield;
                    break;
                case Item.Invincible:
                    if (Invincible == null)
                    {
                        Debug.Log("Material for item " + capItem.ToString() + " has not been assigned.");
                        return;
                    }
                    ItemRenderer.material = Invincible;
                    break;
                default:
                    break;
            }

            if (Application.isPlaying)
            {
                Transform ItemTrans = ItemRenderer.transform;
                Vector3 ItemRot = Quaternion.FromToRotation(transform.forward, transform.up) * RingManager.RingEulers;
                ItemTrans.localRotation = Quaternion.Euler(ItemRot + new Vector3(-90, 0, 0));

                //Set bobbing
                Vector3 tmpPos = InitialPos;
                tmpPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * BobSpeed) * BobAmplitude;
                tmpPos.y += yOffset;
                ItemTrans.localPosition = tmpPos;
            }

        }

        //Enable and disable the DisableOnHit objects depending on if we are active
        foreach (GameObject g in DisableWhenHit)
        {
            g.SetActive(IsActive);
        }
    }
}
