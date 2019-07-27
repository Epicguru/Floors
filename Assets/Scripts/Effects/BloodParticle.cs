using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

[RequireComponent(typeof(PoolObject))]
public class BloodParticle : MonoBehaviour
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

    public LayerMask Mask;
    public Vector3 Velocity;
    public Vector2 RandomSize = new Vector2(0.05f, 0.35f);
    public float Size;
    public DecalProjectorComponent Decal;
    public float GravityScale = 2f;

    private void Update()
    {
        Velocity += Physics.gravity * Time.deltaTime * GravityScale;

        Vector3 oldPos = transform.position;
        Vector3 newPos = transform.position + Velocity * Time.deltaTime;
        transform.localScale = new Vector3(Size, Size, Size) * 0.3f;

        bool didHit = Physics.Linecast(oldPos, newPos, out RaycastHit hit, Mask, QueryTriggerInteraction.Ignore);
        if (didHit)
        {
            const float DST = 0.1f;

            var decal = PoolObject.Spawn(Decal);
            decal.transform.position = hit.point + hit.normal * DST;
            decal.transform.forward = -hit.normal;
            decal.size = new Vector3(Size, Size, 1f);
            decal.transform.SetParent(hit.transform, true);

            PoolObject.Despawn(this);
            CancelInvoke("Despawn");
        }
        else
        {
            transform.position = newPos;
        }
    }

    private void UponSpawn()
    {
        Invoke("Despawn", 10f);
        Size = Random.Range(RandomSize.x, RandomSize.y);
        transform.localScale = new Vector3(Size, Size, Size) * 0.3f;
    }

    private void Despawn()
    {
        PoolObject.Despawn(this);
    }
}
