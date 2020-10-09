using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
[ExecuteInEditMode]
public class CheckpointActor : MonoBehaviour
{
    public bool IsActive;
    BoxCollider col;
    public Animator animator;
    public Transform JointL, JointR; //Both sides of the checkpoint, for width positioning
    [Range(2.5f, 7.5f)] public float Width = 2.5f;

    //Renderers and such for setting when the checkpoint is passed
    public Renderer bulbRenderer;
    public Renderer laserRenderer;
    public Material MatNormal, MatActive;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        //Set collider size
        Vector3 Size = col.size;
        Size.x = Width;
        col.size = Size;
        if (bulbRenderer == null || MatNormal == null || MatActive == null)
            return;
        bulbRenderer.material = (IsActive ? MatActive : MatNormal);
        laserRenderer.enabled = !IsActive;
        animator.SetBool("Passed", IsActive);
    }

    private void LateUpdate()
    {
        if (JointL != null && JointR != null)
        {
            //Set the width of the actual checkpoint
            Vector3 PosL = JointL.localPosition;
            Vector3 PosR = JointR.localPosition;
            float XPos = Width / 2;
            PosL.x = XPos;
            PosR.x = -XPos;
            JointL.localPosition = PosL;
            JointR.localPosition = PosR;
        }

    }
}
