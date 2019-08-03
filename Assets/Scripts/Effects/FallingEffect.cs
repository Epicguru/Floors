using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolObject))]
public class FallingEffect : MonoBehaviour
{
    public PoolObject PoolObject
    {
        get
        {
            if (_po == null)
                _po = GetComponent<PoolObject>();
            return _po;
        }
    }
    private PoolObject _po;

    [Header("Movement")]
    public LayerMask Mask;
    public Vector3 Velocity;
    public Vector3 AngularVel;
    public float GravityScale = 1f;

    [Header("Bouncing")]
    public float BounceCoefficient = 0.4f;
    public float MinBounceTriggerVel = 0.1f;

    private void Update()
    {
        Vector3 start = transform.position;
        Vector3 end = start + Velocity * Time.deltaTime;

        if(DoesIntersect(start, end, out Vector3 normal))
        {
            // Bounce!
            Vector3 newVel = Vector3.Reflect(Velocity, normal) * BounceCoefficient;

            if(Velocity.sqrMagnitude >= MinBounceTriggerVel)
                UponBounce(Velocity, newVel);

            Velocity = newVel;
        }
        else
        {
            transform.position = end;
            transform.Rotate(AngularVel * Time.deltaTime, Space.Self);
        }

        Velocity += Time.deltaTime * Physics.gravity * GravityScale;
    }

    private void UponBounce(Vector3 oldVel, Vector3 newVel)
    {
        // Play some kind of sound or something.

        // For now, slow down rotation.
        AngularVel *= -BounceCoefficient;
    }

    private bool DoesIntersect(Vector3 start, Vector3 end, out Vector3 normal)
    {
        if(Physics.Linecast(start, end, out RaycastHit hit, Mask))
        {
            normal = hit.normal;
            return true;
        }
        else
        {
            normal = Vector3.zero;
            return false;
        }
    }
}
