using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
public class Pawn : MonoBehaviour
{
    private const float KNOCKBACK_UPS = 20f;
    private static List<Item> tempItems = new List<Item>();

    public Health Health
    {
        get
        {
            if (_health == null)
                _health = GetComponent<Health>();
            return _health;
        }
    }
    private Health _health;
    public CharacterController CharacterController
    {
        get
        {
            if (_cc == null)
                _cc = GetComponent<CharacterController>();
            return _cc;
        }
    }
    private CharacterController _cc;
    public NavMeshAgent Nav
    {
        get
        {
            if (_nma == null)
                _nma = GetComponent<NavMeshAgent>();
            return _nma;
        }
    }
    private NavMeshAgent _nma;
    public PawnRagdoll PawnRagdoll
    {
        get
        {
            if (_ragdoll == null)
                _ragdoll = GetComponentInChildren<PawnRagdoll>();
            return _ragdoll;
        }
    }
    private PawnRagdoll _ragdoll;

    public string Name = "Pawn";
    public bool IsBot = true;
    public Transform BotTarget;
    public float Speed = 5;
    [Range(0f, 1f)]
    public float KnockbackRecoveryFactor = 0.8f;
    public float DeathKnockbackCoefficient = 0.15f;

    [Header("Item pickup")]
    public float PickupRange = 1.5f;

    public Item Item
    {
        get
        {
            return _item;
        }
        set
        {
            SetItem(value);
        }
    }
    private Item _item;

    [Header("References")]
    public Transform ItemParent;

    [Header("Input")]
    public Vector2 DirectionInput;
    public float Rotation;

    private Vector3 knockback;
    [SerializeField]
    private Item tempItem;

    private void Awake()
    {
        if (tempItem != null)
            Item = tempItem;

        InvokeRepeating("UpdateKnockback", 0f, 1f / KNOCKBACK_UPS);
    }

    private void Update()
    {
        var list = GetItemsInPickupRange();
        if(list.Count > 0)
        {
            var item = list[0];
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SetItem(item);
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsBot)
        {
            BotBehaviour();
        }

        if (Health.IsDead)
            return;

        transform.localRotation = Quaternion.Euler(0f, Rotation, 0f);

        // Normalize the input. This sucks for handheld controllers, but it will do for now.
        var input = DirectionInput.normalized;

        Vector3 final = Vector3.zero;

        final += new Vector3(input.x, -1f, input.y);
        final *= Speed;

        final += knockback;

        final *= Time.deltaTime;

        CharacterController.Move(final);
    }

    private void BotBehaviour()
    {
        if (Health.IsDead)
            return;

        if(BotTarget != null)
            Nav.SetDestination(BotTarget.position);

        Vector3 velocity = Nav.desiredVelocity.normalized;
        DirectionInput.x = velocity.x;
        DirectionInput.y = velocity.z;

        Rotation = 90f - new Vector2(Nav.desiredVelocity.x, Nav.desiredVelocity.z).ToAngle();
    }

    public void AddKnockback(Vector3 amount)
    {
        this.knockback += amount;
    }

    private void UpdateKnockback()
    {
        knockback *= KnockbackRecoveryFactor;
    }

    private void SetItem(Item i)
    {
        if (_item == i)
            return;

        if(_item != null)
        {
            _item.Pawn = null;
            _item.transform.SetParent(null);
        }

        i.Pawn = this;
        i.transform.SetParent(ItemParent);
        i.transform.localPosition = Vector3.zero;
        i.transform.localRotation = Quaternion.identity;
        _item = i;
    }

    public List<Item> GetItemsInPickupRange()
    {
        if (tempItems == null)
            tempItems = new List<Item>();
        tempItems.Clear();

        float maxSqrDst = PickupRange * PickupRange;

        foreach (var item in Item.DroppedItems)
        {
            if (!item.IsDropped)
                continue;

            float cx = item.transform.position.x - transform.position.x;
            float cz = item.transform.position.z - transform.position.z;

            float sqrDst = cx * cx + cz * cz;
            if(sqrDst <= maxSqrDst)
            {
                tempItems.Add(item);
            }
        }

        return tempItems;
    }

    public void OnHealthChange(DamageInfo info)
    {
        if (Health.IsDead)
        {
            PawnRagdoll.Enable(info.IncomingDirection * info.HealthChange * -DeathKnockbackCoefficient);
            Destroy(this.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (IsBot)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + Vector3.up * 2f, transform.position + Vector3.up * 2f + Nav.desiredVelocity * 2f);
        }
    }
}
