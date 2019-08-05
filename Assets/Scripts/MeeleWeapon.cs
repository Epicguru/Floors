
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class MeeleWeapon : MonoBehaviour
{
    private const int MAX_HITS = 32;
    private static readonly Collider[] BoxHits = new Collider[MAX_HITS];
    private static readonly List<(Health, Pawn)> tempColliders = new List<(Health, Pawn)>();

    public Item Item
    {
        get
        {
            if (_item == null)
                _item = GetComponent<Item>();
            return _item;
        }
    }
    private Item _item;
    public Animator Animator
    {
        get
        {
            if (_anim == null)
                _anim = GetComponentInChildren<Animator>();
            return _anim;
        }
    }
    private Animator _anim;

    [Header("Hitboxes")]
    public BoxCollider LightHitbox;
    public BoxCollider HeavyHitbox;

    [Header("Damage")]
    public float LightDamage = 40f;
    public float HeavyDamage = 80f;
    public float Knockback = 2f;

    [Header("Lunge")]
    public float LungeStrength = 1.5f;

    [Header("Takedown")]
    public bool CanDoTakedown = false;
    public Vector3 TakedownEnemyPos = new Vector3(0f, 0f, 1f);
    public Vector3 TakedownEnemyDirection = new Vector3(0f, 0f, -1f);

    [Header("Gore")]
    public int BloodCount = 10;
    public float BloodVelocity = 6f;
    public Vector3 BloodRandomness = new Vector3(5f, 2f, 5f);
    [Range(0f, 1f)]
    public float LightDecapChance = 0f;
    [Range(0f, 1f)]
    public float HeavyDecapChance = 0.1f;

    [Header("Input")]
    public bool LightAttack;
    public bool HeavyAttack;
    public bool Safe;

    public int AttackCounter { get { return LightAttackCounter + HeavyAttackCounter; } }
    public int LightAttackCounter { get; private set; }
    public int HeavyAttackCounter { get; private set; }

    private void Awake()
    {
        if(LightHitbox == null && HeavyHitbox == null)
        {
            Debug.LogError($"Meele weapon {Item.Name} has no hitboxes. Assign at least one light or heavy hitbox.");
        }
        else
        {
            if(LightHitbox == null)
            {
                LightHitbox = HeavyHitbox;
                Debug.LogWarning($"Meele weapon {Item.Name} only has a heavy hitbox assigned, heavy hitbox will be used for light attacks too. To remove this warning, manually assign the light hitbox.");
            }
            if(HeavyHitbox == null)
            {
                HeavyHitbox = LightHitbox;
                Debug.LogWarning($"Meele weapon {Item.Name} only has a light hitbox assigned, light hitbox will be used for heavy attacks too. To remove this warning, manually assign the heavy hitbox.");
            }
        }

        if (LightHitbox != null)
            LightHitbox.isTrigger = true;
        if (HeavyHitbox != null)
            HeavyHitbox.isTrigger = true;
    }

    private void Update()
    {
        Animator.SetBool("Safe", Safe);
        Animator.SetBool("Attack", !Safe && (LightAttack || HeavyAttack));
        Animator.SetBool("Heavy", HeavyAttack);

        UpdateTakedown();
    }

    private void UpdateTakedown()
    {
        if (Item.IsDropped)
            return;

        if (Safe)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            var list = GetCollisionPawns(LightHitbox);

            foreach ((Health h, Pawn p) in list)
            {
                if (p == null)
                    continue;

                if (p == Item.Pawn)
                    continue;

                if (Item.Pawn.CanPerformTakedown(p))
                {
                    Item.Pawn.PerformTakedownOn(p);
                    break;
                }
            }

            list.Clear();
        }    
    }

    private void UponAnimationEvent(AnimationEvent e)
    {
        string s = e.stringParameter.Trim().ToLower();
        switch (s)
        {
            case "light":

                HitObjects(LightHitbox, LightDamage, Random.value <= LightDecapChance);
                LightAttackCounter++;

                break;

            case "heavy":

                HitObjects(HeavyHitbox, HeavyDamage, Random.value <= HeavyDecapChance);
                HeavyAttackCounter++;

                break;

            case "lunge":

                Item.Pawn.AddKnockback(Item.transform.forward * LungeStrength);

                break;

            case "takedown":

                if(Item.Pawn?.TakedownPawn != null)
                    TakedownKill(Item.Pawn.TakedownPawn, e.intParameter > 0);

                break;
        }
    }

    private static List<(Health, Pawn)> GetCollisionPawns(BoxCollider c)
    {
        tempColliders.Clear();

        Vector3 center = c.transform.TransformPoint(c.center);
        Vector3 scale = c.transform.lossyScale;
        Vector3 size = new Vector3(scale.x * c.size.x, scale.y * c.size.y, scale.z * c.size.z);
        int hitCount = Physics.OverlapBoxNonAlloc(center, size * 1f, BoxHits, c.transform.rotation);

        if (hitCount > MAX_HITS)
        {
            Debug.LogWarning($"Hit {hitCount} colliders on meele sweep, but only {MAX_HITS} are supported.");
            hitCount = MAX_HITS;
        }

        for (int i = 0; i < hitCount; i++)
        {
            var collider = BoxHits[i];

            if (collider.isTrigger)
                continue;

            var pawn = collider.GetComponentInParent<Pawn>();
            var health = pawn != null ? pawn.Health : collider.GetComponentInParent<Health>();

            if (health == null)
                continue;

            tempColliders.Add((health, pawn));
        }

        return tempColliders;
    }

    private void HitObjects(BoxCollider c, float damage, bool decap)
    {
        var list = GetCollisionPawns(c);

        foreach (var pair in list)
        {
            (Health health, Pawn pawn) = pair;

            if (pawn != null)
            {
                if (pawn == Item.Pawn)
                    continue;

                if (!pawn.Health.IsDead)
                {
                    Vector3 dir = (pawn.transform.position - transform.position).normalized;
                    pawn.AddKnockback(dir * Knockback);
                }
            }

            if (health != null && !health.IsDead)
            {
                DamageInfo info = new DamageInfo(-damage, Item.Name, "Meele damage.");
                info.IncomingDirection = (health.transform.position - Item.Pawn.transform.position).normalized;
                info.ShouldDecap = decap;
                health.ChangeHealth(info);

                OnDealDamage(health, pawn, info.IncomingDirection);
            }
        }

        list.Clear();   
    }

    private void OnDealDamage(Health h, Pawn p, Vector3 direction)
    {
        if (p == null)
            return;

        for (int i = 0; i < BloodCount; i++)
        {
            var blood = PoolObject.Spawn(Spawnables.BloodParticle);
            var r = BloodRandomness;
            var rand = new Vector3(Random.Range(-r.x, r.x), Random.Range(-r.y, r.y), Random.Range(-r.z, r.z));
            blood.transform.position = h.transform.position + Vector3.up * 1f + direction * 0.8f;
            blood.Velocity = direction.normalized * BloodVelocity + rand;
        }
    }

    private void TakedownKill(Pawn pawn, bool decap)
    {
        // Kill the takedown enemy using this meele weapon. The animation has already been played.
        DamageInfo info = new DamageInfo(-pawn.Health.CurrentHealth, Item.Name, "Meele takedown damage.");
        info.ShouldDecap = decap;
        pawn.Health.ChangeHealth(info);
    }
}
