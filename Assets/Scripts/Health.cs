using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        private set
        {
            _maxHealth = value;
        }
    }
    [SerializeField]
    private float _maxHealth = 100f;

    public float CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        private set
        {
            _currentHealth = value;
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);
        }
    }
    [SerializeField]
    private float _currentHealth = 100f;

    public bool IsDead
    {
        get
        {
            return CurrentHealth == 0f;
        }
    }

    public void ChangeHealth(DamageInfo info)
    {
        if (IsDead)
        {
            Debug.LogWarning("Health is already dead. Cannot change health.");
            return;
        }

        CurrentHealth += info.HealthChange;
        BroadcastMessage("OnHealthChange", info, SendMessageOptions.DontRequireReceiver);
    }
}
