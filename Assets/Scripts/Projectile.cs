using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolObject))]
public class Projectile : MonoBehaviour
{
    public LayerMask CollisionMask;
    public float Speed = 2f;
    public Vector3 Direction = Vector3.forward;
    public float KnockbackMultiplier = 1f;
    //public BloodParticle Blood;
    public int BloodSpawnCount = 5;
    public float BloodSpawnVelocity = 7f;
    public Vector3 BloodSpawnRandomness = new Vector3(2f, 1f, 2f);
    public float Damage = 25f;

    private void Update()
    {
        MoveAndCollide(transform.position, transform.position + Direction.normalized * Speed * Time.deltaTime);
    }

    private void MoveAndCollide(Vector3 oldPos, Vector3 newPos)
    {
        bool didHit = Physics.Linecast(oldPos, newPos, out RaycastHit hit, CollisionMask, QueryTriggerInteraction.Ignore);

        if (didHit)
        {
            PoolObject.Despawn(this);
            Pawn p = hit.transform.GetComponentInParent<Pawn>();
            Health h = hit.transform.GetComponentInParent<Health>();
            if (p != null)
            {
                p.AddKnockback(Direction * KnockbackMultiplier);
            }

            UponCollision(hit, p, h);
        }
        else
        {
            transform.position = newPos;
        }
    }

    private void UponCollision(RaycastHit hit, Pawn pawn, Health health)
    {
        if(pawn != null)
        {
            for (int i = 0; i < BloodSpawnCount; i++)
            {
                var blood = PoolObject.Spawn(Spawnables.BloodParticle);
                blood.transform.position = hit.point + hit.normal * 0.1f;
                blood.Velocity = hit.normal * BloodSpawnVelocity;
                var r = BloodSpawnRandomness;
                var random = new Vector3(Random.Range(-r.x, r.x), Random.Range(-r.y, r.y), Random.Range(-r.z, r.z));
                blood.Velocity += random;
            }
        }
        else
        {
            var go = PoolObject.Spawn(Spawnables.SparksEffect);
            go.transform.position = hit.point + hit.normal * 0.01f;
            go.transform.forward = hit.normal;
        }

        if(health != null)
        {
            DamageInfo info = new DamageInfo(-Damage, "projectile", "Projectile damage", hit)
            {
                IncomingDirection = Direction
            };
            health.ChangeHealth(info);
        }
    }
}
