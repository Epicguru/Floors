using UnityEngine;

public class PawnRagdoll : MonoBehaviour
{
    public Rigidbody[] Bodies;
    public Collider[] Colliders;

    public void EnableDecap(Vector3 bodyForce, Vector3 headForce, Vector3 headRot, bool blood = true)
    {
        foreach (var collider in Colliders)
        {
            collider.enabled = true;
            collider.isTrigger = false;
        }
        foreach (var body in Bodies)
        {
            body.isKinematic = false;

            if(body.gameObject.name == "Head")
            {
                body.AddForce(headForce, ForceMode.Impulse);
                body.AddTorque(headRot, ForceMode.Impulse);
                Destroy(body.gameObject.GetComponent<HingeJoint>());

                if (blood)
                {
                    body.gameObject.GetComponent<BloodSpurt>().Active = true;
                }
            }
            else
            {
                body.AddForce(bodyForce, ForceMode.Impulse);
            }
        }

        transform.SetParent(null, true);
    }

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
