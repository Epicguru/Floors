using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : StateMachineBehaviour
{
    private Pawn pawn;

    private void CheckPawn(Animator a)
    {
        if (pawn == null)
            pawn = a.GetComponent<Pawn>();
    }

    public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CheckPawn(animator);
        OnEnter(animator, pawn);
    }
    public abstract void OnEnter(Animator anim, Pawn pawn);

    public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnUpdate(animator, pawn);
    }
    public abstract void OnUpdate(Animator anim, Pawn pawn);

    public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnExit(animator, pawn);
    }
    public abstract void OnExit(Animator anim, Pawn pawn);
}
