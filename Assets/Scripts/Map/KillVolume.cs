using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillVolume : MonoBehaviour
{
    public bool UseFlatDamage = false;

    public float FlatDamage = 100f;
    [Range(0f, 1f)]
    public float HealthPercentage = 1f;

    private void OnTriggerEnter(Collider other)
    {
        Health h = other.GetComponentInParent<Health>();
        if (h == null)
            return;
        if (h.IsDead)
            return;

        float change = 0f;
        if (UseFlatDamage)
        {
            change = -FlatDamage;
        }
        else
        {
            change = -h.MaxHealth * HealthPercentage;
        }

        DamageInfo info = new DamageInfo(change, "The void", "Damage from the void. There is no escape...");
        info.HitPoint = other.transform.position;

        h.ChangeHealth(info);
    }
}
