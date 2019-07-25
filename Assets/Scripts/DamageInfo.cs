
using UnityEngine;

public struct DamageInfo
{
    public string Dealer;
    public float HealthChange;
    public string Description;
    public Vector3 IncomingDirection;
    public Vector3 HitPoint;
    public Vector3 HitNormal;

    public DamageInfo(float change, string dealer = null, string description = null)
    {
        this.HealthChange = change;
        this.Dealer = dealer;
        this.Description = description;
        this.IncomingDirection = Vector3.zero;
        this.HitPoint = Vector3.zero;
        this.HitNormal = Vector3.zero;
    }

    public DamageInfo(float change, string dealer, string description, RaycastHit hit)
        :this(change, dealer, description)
    {
        this.HitPoint = hit.point;
        this.HitNormal = hit.normal;
    }
}
