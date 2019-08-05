using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
[ExecuteInEditMode]
public class Pawn : MonoBehaviour
{
    public static List<Pawn> AllAlive = new List<Pawn>();
    private static List<Item> tempItems = new List<Item>();
    private const float KNOCKBACK_UPS = 20f;

    public AIPawnController AIPawnController
    {
        get
        {
            if (_aiController == null)
                _aiController = GetComponent<AIPawnController>();
            return _aiController;
        }
    }
    private AIPawnController _aiController;
    public PlayerPawnController PlayerPawnController
    {
        get
        {
            if (_playerController == null)
                _playerController = GetComponent<PlayerPawnController>();
            return _playerController;
        }
    }
    private PlayerPawnController _playerController;
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
    public Animator StateMachineAnimator
    {
        get
        {
            if (_anim == null)
                _anim = GetComponent<Animator>();
            return _anim;
        }
    }
    private Animator _anim;
    public bool IsDead
    {
        get
        {
            return Health.IsDead;
        }
    }

    public string Name = "Pawn";

    [Header("AI")]
    public bool IsBot = true;
    public Vector3 BotTargetPos;
    public float BotSlowdownDistance = 1f;
    public AnimationCurve BotSlowdownCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    public bool BotAtTarget
    {
        get
        {
            const float DST = 0.5f;
            return (transform.position - BotTargetPos).sqrMagnitude <= (DST * DST);
        }
    }

    [Header("Movement")]
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
    public Transform Graphics;

    [Header("Takedown")]
    public Pawn TakedownPawn;
    public Vector3 TakedownEnemyPos
    {
        get
        {
            if(Item != null)
            {
                if (Item.IsMeeleWeapon)
                {
                    return Item.MeeleWeapon.TakedownEnemyPos;
                }

                // TODO allow guns to do takedowns here.

                return Vector3.forward;
            }
            else
            {
                return Vector3.forward;
            }
        }
    }
    public Vector3 TakedownEnemyDirection
    {
        get
        {
            if (Item != null)
            {
                if (Item.IsMeeleWeapon)
                {
                    return Item.MeeleWeapon.TakedownEnemyDirection;
                }

                // TODO allow guns to do takedowns here.

                return Vector3.forward;
            }
            else
            {
                return Vector3.forward;
            }
        }
    }

    [Header("Input")]
    public Vector2 DirectionInput;
    public float Rotation;

    [Header("Takedown")]
    public bool InTakedown = false;
    [Range(0f, 1f)]
    public float TakedownLerp;
    public Pawn TakenDownBy;

    [Header("Other")]
    public Room CurrentRoom;
    public Vector3 GraphicsPositionOffset;
    public Vector3 GraphicsAngleOffset;

    private Vector3 knockback;
    [SerializeField]
    private Item tempItem;
    private float botSpeedMulti = 1f;
    private Vector3 preTakedownPos;
    private Quaternion preTakedownRot;
    private float takedownTimer;
    private bool hasTriggeredTakedownAnim;

    private void Awake()
    {
        if (!Application.isPlaying)
            return;

        if (tempItem != null)
            Item = tempItem;

        InvokeRepeating("UpdateKnockback", 0f, 1f / KNOCKBACK_UPS);
        AllAlive.Add(this);
        ClearBotTarget();
    }

    private void Update()
    {
        if(Graphics != null)
        {
            Graphics.localPosition = GraphicsPositionOffset;
            Graphics.localEulerAngles = GraphicsAngleOffset;
        }

        if (!Application.isPlaying)
            return;

        if (InTakedown)
        {
            if(TakedownPawn != null)
            {
                takedownTimer += Time.deltaTime;
                const float TAKEDOWN_LERP_TIME = 0.1f;

                float p = takedownTimer / TAKEDOWN_LERP_TIME;
                p = Mathf.Clamp01(p);

                TakedownPawn.TakedownLerp = p;

                if(p == 1f && !hasTriggeredTakedownAnim)
                {
                    hasTriggeredTakedownAnim = true;
                    Item.Animator.SetTrigger("Takedown");

                    // TODO handle when the active item suddely disapears.
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!Application.isPlaying)
            return;

        if (IsBot)
        {
            BotBehaviour();
        }

        if (Health.IsDead)
            return;        

        // Normalize the input. This sucks for handheld controllers, but it will do for now.
        var input = DirectionInput.normalized;

        Vector3 final = Vector3.zero;

        final += new Vector3(input.x, -1f, input.y);
        final *= Speed;
        if (IsBot)
            final *= botSpeedMulti;

        final += knockback;

        final *= Time.deltaTime;

        if (!InTakedown)
        {
            CharacterController.Move(final);
            transform.localRotation = Quaternion.Euler(0f, Rotation, 0f);
        }
        else if(TakenDownBy != null)
        {
            Vector3 targetPos = TakenDownBy.transform.TransformPoint(TakenDownBy.TakedownEnemyPos);
            transform.position = Vector3.Lerp(preTakedownPos, targetPos, TakedownLerp);

            Vector3 dir = TakenDownBy.TakedownEnemyDirection.normalized;
            Vector3 finalDir = TakenDownBy.transform.forward * dir.z;
            finalDir += TakenDownBy.transform.right * dir.x;

            Quaternion targetRot = Quaternion.LookRotation(finalDir);
            Quaternion oldRot = preTakedownRot;

            // Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + finalDir, Color.red, 0.2f);

            transform.rotation = Quaternion.Lerp(oldRot, targetRot, TakedownLerp);
        }
    }

    public bool CanPerformTakedown(Pawn p)
    {
        if (p == null)
            return false;

        if (InTakedown || p.InTakedown)
            return false;

        if (IsDead || p.IsDead)
            return false;

        if (Item != null)
        {
            if (Item.IsMeeleWeapon)
            {
                return Item.MeeleWeapon.CanDoTakedown;
            }

            return false;
        }
        else
        {
            return false;
        }
    }

    public void PerformTakedownOn(Pawn pawn)
    {
        if(pawn == null)
        {
            Debug.LogError("Cannot takedown null pawn.");
            return;
        }

        if(pawn == this)
        {
            Debug.LogError("Cannot takedown self! You are worth it!");
            return;
        }

        if (InTakedown)
        {
            Debug.LogError("Already in takedown. Wait a little.");
            return;
        }

        if (pawn.InTakedown)
        {
            Debug.LogError("Target pawn is already in takedown.");
            return;
        }

        // Check meele weapon for takedown capability.
        if(Item != null)
        {
            if (Item.IsMeeleWeapon)
            {
                if (!Item.MeeleWeapon.CanDoTakedown)
                {
                    Debug.LogWarning($"Meele weapon {Item.Name} does not support performing takedowns.");
                    return;
                }
            }
            else
            {
                Debug.LogWarning($"Item {Item.Name} does not support performing takedowns.");
                return;
            }
        }
        else
        {
            Debug.LogError("Cannot perform takedown with your bare hands! Yet...");
            return;
        }

        takedownTimer = 0f;
        InTakedown = true;
        hasTriggeredTakedownAnim = false;
        pawn.InTakedown = true;
        pawn.TakenDownBy = this;
        pawn.TakedownLerp = 0f;
        pawn.preTakedownPos = pawn.transform.position;
        pawn.preTakedownRot = pawn.transform.rotation;
        this.TakedownPawn = pawn;
    }

    /// <summary>
    /// Stop bot movement by setting the target position to it's current position.
    /// </summary>
    public void ClearBotTarget()
    {
        BotTargetPos = transform.position;
    }

    private void BotBehaviour()
    {
        if (Health.IsDead)
            return;

        if(Nav.pathStatus == NavMeshPathStatus.PathPartial)
        {
            //Debug.LogWarning("Cleared bot partial path.");
            ClearBotTarget();
        }

        Nav.SetDestination(BotTargetPos);

        Vector3 diff = (transform.position - BotTargetPos);
        float sqr = diff.sqrMagnitude;
        if(sqr <= BotSlowdownDistance * BotSlowdownDistance)
        {
            float real = diff.magnitude;
            float p = 1f - (real / BotSlowdownDistance);
            float x = BotSlowdownCurve.Evaluate(p);

            botSpeedMulti = x;
        }
        else
        {
            botSpeedMulti = 1f;
        }

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
            if (info.ShouldDecap)
            {
                Vector3 normal = info.IncomingDirection * info.HealthChange * -DeathKnockbackCoefficient;
                Vector3 randomTorque = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 5.0f), Random.Range(-50f, 50f));
                PawnRagdoll.EnableDecap(normal, normal * 0.2f + new Vector3(0f, 6f, 0f), randomTorque, true);
            }
            else
            {
                PawnRagdoll.Enable(info.IncomingDirection * info.HealthChange * -DeathKnockbackCoefficient);
            }
            Destroy(this.gameObject);
        }
    }

    private void UponAnimationEvent(AnimationEvent e)
    {
        string str = e.stringParameter.Trim().ToLower();

        switch (str)
        {
            case "takedown":

                InTakedown = false;
                TakedownPawn = null;
                hasTriggeredTakedownAnim = false;
                Debug.Log("Finished takedown!");

                break;
        }
    }

    private void RemoveFromRooms()
    {
        if (AllAlive.Contains(this))
            AllAlive.Remove(this);
        if (CurrentRoom != null)
            if (CurrentRoom.Pawns.Contains(this))
                CurrentRoom.Pawns.Remove(this);
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying)
            return;

        RemoveFromRooms();
    }

    private void OnDrawGizmos()
    {
        if (IsBot && Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + Vector3.up * 2f, transform.position + Vector3.up * 2f + Nav.desiredVelocity * 2f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + Vector3.up * 2f, BotTargetPos);
            Gizmos.DrawCube(BotTargetPos, Vector3.one * 0.7f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + Vector3.up * 2f, Nav.pathEndPosition);
            Gizmos.DrawCube(Nav.pathEndPosition, Vector3.one * 0.3f);
        }
    }
}
