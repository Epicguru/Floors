
using UnityEngine;
using UnityEngine.AI;

public class State_Wander : AIState
{
    private Vector2 PauseTime = new Vector2(1f, 2f);
    private Vector2 Distance = new Vector2(4f, 10f);
    private float MaxPathTime = 15f;

    private bool waiting;
    private float timeToPause;
    private float timer;

    public override void OnEnter(Animator anim, Pawn pawn)
    {
        pawn.ClearBotTarget();
        waiting = false;
        timer = 0f;
    }

    private Vector3 RandomPoint(Pawn p)
    {
        const int MAX_TRIES = 20;
        for (int i = 0; i < MAX_TRIES; i++)
        {
            float dst = Random.Range(Distance.x, Distance.y);
            Vector2 circle = Random.insideUnitCircle.normalized * dst;
            Vector3 pos = new Vector3(circle.x, p.transform.position.y, circle.y);
            pos += p.transform.position;

            if(NavMesh.SamplePosition(pos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return p.transform.position;
    }

    public override void OnUpdate(Animator anim, Pawn pawn)
    {
        if (pawn.BotAtTarget)
        {
            // Wait for a second or two...
            if (!waiting)
            {
                waiting = true;
                timer = 0f;
                timeToPause = Random.Range(PauseTime.x, PauseTime.y);
            }
        }
        else
        {
            // Move and check path time.
            timer += Time.deltaTime;
            if(timer >= MaxPathTime)
            {
                // Moving to this new position has taken too long, select a new position.
                pawn.ClearBotTarget();
                return; // Return to wait until next frame to select waiting time.
            }
        }

        if (waiting)
        {
            timer += Time.deltaTime;
            if(timer >= timeToPause)
            {
                timer = 0f;
                waiting = false;

                // Select new destination.
                float dst = Random.Range(Distance.x, Distance.y);
                Vector2 circle = Random.insideUnitCircle.normalized * dst;
                Vector3 pos = new Vector3(circle.x, pawn.transform.position.y, circle.y);

                pawn.BotTargetPos = RandomPoint(pawn);
            }
        }
    }

    public override void OnExit(Animator anim, Pawn pawn)
    {
        pawn.ClearBotTarget();
    }
}
