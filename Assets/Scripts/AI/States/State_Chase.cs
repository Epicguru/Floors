using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Chase : AIState
{
    public override void OnEnter(Animator anim, Pawn pawn)
    {

    }

    public override void OnUpdate(Animator anim, Pawn pawn)
    {
        // Check if we have an enemy to target. If we don't (such as loosing sight) then change the state.
        if(pawn.AIPawnController.TargetEnemy != null)
        {
            pawn.BotTargetPos = pawn.AIPawnController.TargetEnemy.transform.position;
        }
        else
        {
            // Go to wander state.
            anim.SetBool("Chase", false);
            anim.SetBool("Wander", true);
        }
    }

    public override void OnExit(Animator anim, Pawn pawn)
    {
        pawn.ClearBotTarget();
    }
}
