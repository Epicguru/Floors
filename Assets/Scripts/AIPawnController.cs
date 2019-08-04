using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class AIPawnController : MonoBehaviour
{
    public Pawn Pawn
    {
        get
        {
            if (_pawn == null)
                _pawn = GetComponent<Pawn>();
            return _pawn;
        }
    }
    private Pawn _pawn;

    /// <summary>
    /// The target that the AI has selected to chase, attack or run away from. May be null.
    /// </summary>
    public Pawn TargetEnemy;

    public float AttackRange = 1.5f;
    [Range(0f, 1f)]
    public float HeavyAttackChance = 0.333f;

    private int lastAttackCounter;
    private bool chooseHeavy = false;

    private void Update()
    {
        if (Pawn == null)
            return;

        Pawn.IsBot = true;
        UpdateMeele();
        UpdateGun();
    }

    private void UpdateMeele()
    {
        MeeleWeapon m = Pawn.Item?.MeeleWeapon;
        if (m == null)
            return;

        if (lastAttackCounter != m.AttackCounter)
        {
            chooseHeavy = Random.value <= HeavyAttackChance;
            lastAttackCounter = m.AttackCounter;
        }

        if(TargetEnemy == null)
        {
            m.LightAttack = false;
            m.HeavyAttack = false;
        }
        else
        {
            Vector3 targetPos = TargetEnemy.transform.position;
            Vector3 botPos = Pawn.transform.position;

            float sqrDst = (targetPos - botPos).sqrMagnitude;
            if(sqrDst <= AttackRange * AttackRange)
            {
                m.HeavyAttack = chooseHeavy;
                m.LightAttack = !chooseHeavy;
            }
            else
            {
                m.LightAttack = false;
                m.HeavyAttack = false;
            }
        }
    }

    private void UpdateGun()
    {
        // TODO implement
    }
}
