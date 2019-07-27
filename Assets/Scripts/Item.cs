using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Item : MonoBehaviour
{
    public static List<Item> DroppedItems = new List<Item>();

    [Header("Info")]
    public string Name;
    public Pawn Pawn;

    [Header("Dropped Behaviour")]
    public float ColliderSize = 0.4f;
    public bool DoDroppedRotation = true;

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
    public Rigidbody Rigidbody
    {
        get
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody>();
            return _rb;
        }
    }
    private Rigidbody _rb;
    public SphereCollider SphereCollider
    {
        get
        {
            if (_collider == null)
                _collider = GetComponent<SphereCollider>();
            return _collider;
        }
    }
    private SphereCollider _collider;

    public bool IsGun { get { return Gun != null; } }
    public Gun Gun
    {
        get
        {
            if (_gun == null)
                _gun = GetComponent<Gun>();
            return _gun;
        }
    }
    private Gun _gun;

    public bool IsMeeleWeapon { get { return MeeleWeapon != null; } }
    public MeeleWeapon MeeleWeapon
    {
        get
        {
            if (_meele == null)
                _meele = GetComponent<MeeleWeapon>();
            return _meele;
        }
    }
    private MeeleWeapon _meele;

    public bool IsDropped
    {
        get
        {
            return Pawn == null;
        }
    }

    private Transform graphics;
    private float floatTimer;

    private void Awake()
    {
        graphics = transform.Find("Graphics");
        if(graphics == null)
        {
            Debug.LogError("Item is missing a Graphics child. Create a child gameobject called Graphics.");
        }
        DroppedItems.Add(this);
    }

    private void OnDestroy()
    {
        if(DroppedItems.Contains(this))
            DroppedItems.Remove(this);
    }

    private void Update()
    {
        gameObject.layer = 12;

        if(Animator != null)
        {
            Animator.SetBool("Dropped", IsDropped);
        }

        SphereCollider.radius = ColliderSize;
        SphereCollider.isTrigger = !IsDropped;
        Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        if (Rigidbody.isKinematic != !IsDropped)
        {
            Rigidbody.isKinematic = !IsDropped;
        }

        if(DroppedItems.Contains(this) != IsDropped)
        {
            if (IsDropped)
                DroppedItems.Add(this);
            else
                DroppedItems.Remove(this);
        }

        UpdateFloating();        
    }

    public void SetVelocity(Vector3 vel)
    {
        Rigidbody.velocity = vel;
    }

    private void UpdateFloating()
    {
        if (graphics == null)
            return;

        if(!IsDropped || !DoDroppedRotation)
        {
            graphics.localPosition = Vector3.zero;
            graphics.localRotation = Quaternion.identity;
        }
        else
        {
            floatTimer += Time.deltaTime;

            const float AMPLITUDE = 0.3f;
            const float FREQUENCY = 0.5f;
            const float REPEAT = 2 * Mathf.PI;

            float value = Mathf.Sin(floatTimer * REPEAT * FREQUENCY) * AMPLITUDE;
            graphics.localPosition = new Vector3(0f, value, 0f);

            const float TURN_SPEED = 180f;
            graphics.localRotation = Quaternion.Euler(0f, TURN_SPEED * floatTimer, 0f);
        }
    }
}
