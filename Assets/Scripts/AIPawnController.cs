using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPawnController : MonoBehaviour
{
    public Pawn Pawn;
    public float AttackRange = 1.5f;
    [Range(0f, 1f)]
    public float HeavyAttackChance = 0.333f;

    private int lastAttackCounter;
    private bool chooseHeavy = false;

    private void Update()
    {
        if (Pawn == null)
            return;

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

        if(Pawn.BotTarget == null)
        {
            m.LightAttack = false;
            m.HeavyAttack = false;
        }
        else
        {
            Vector3 targetPos = Pawn.BotTarget.transform.position;
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
