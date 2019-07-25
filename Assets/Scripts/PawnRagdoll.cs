using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnRagdoll : MonoBehaviour
{
    public Rigidbody[] Bodies;
    public Collider[] Colliders;

    public void Enable(Vector3 force)
    {
        foreach (var collider in Colliders)
        {
            collider.enabled = true;
            collider.isTrigger = false;
        }
        foreach (var body in Bodies)
        {
            body.isKinematic = false;
            body.AddForce(force, ForceMode.Impulse);
        }

        transform.SetParent(null, true);
    }

    public void Disable()
    {
        foreach (var collider in Colliders)
        {
            collider.enabled = false;
            collider.isTrigger = true;
        }
        foreach (var body in Bodies)
        {
            body.isKinematic = true;
        }
    }
}
