using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class DebugHurtTrigger : MonoBehaviour
{
    public Collider _trigger;
    SphereCollider _sphere;
    BoxCollider _box;
    public Color triggerColor;

    private void OnDrawGizmos()
    {
        if (_trigger != null)
        {
            if (_trigger is SphereCollider)
            {
                _sphere = (SphereCollider)_trigger;
                Gizmos.color = triggerColor;
                Gizmos.DrawSphere(transform.position + _sphere.center, _sphere.radius);
            }
            if (_trigger is BoxCollider)
            {
                _box = (BoxCollider)_trigger;
                Gizmos.color = triggerColor;
                Gizmos.DrawCube(transform.position + _box.center, _box.size);
            }
        }
    }

}
