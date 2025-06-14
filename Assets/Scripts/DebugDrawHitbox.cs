using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDrawHitbox : MonoBehaviour
{
    [SerializeField]
    private SphereCollider sphereCollider;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(sphereCollider.transform.position, sphereCollider.transform.lossyScale.x * sphereCollider.radius);
    }
}
