
using UnityEngine;

public class BloodSpurt : MonoBehaviour
{
    public bool Active = false;
    public Vector3 Offset;
    public Vector3 Direction;
    public float Rate = 5f;
    public float Time = 10f;
    public AnimationCurve RateOverTime = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    public float ParticleSpeed = 1.5f;
    public float ParticleRandomness = 0.2f;

    private float timer;
    private float spawnTimer;

    public Vector3 WorldPos
    {
        get
        {
            return transform.TransformPoint(Offset);
        }
    }

    public Vector3 WorldDirection
    {
        get
        {
            return transform.TransformDirection(Direction).normalized;
        }
    }

    private void Update()
    {
        if (!Active)
            return;

        timer += UnityEngine.Time.deltaTime;
        spawnTimer += UnityEngine.Time.deltaTime;
        float p = timer / Time;
        float x = RateOverTime.Evaluate(p);
        float rate = Rate * x;

        if (spawnTimer > Time)
            return;

        if(spawnTimer >= 1f / rate)
        {
            spawnTimer = 0f;
            Spawn();
        }
    }

    private void Spawn()
    {
        var spawned = PoolObject.Spawn(Spawnables.BloodParticle);
        spawned.transform.position = WorldPos;
        Vector3 random = new Vector3(Random.Range(-ParticleRandomness, ParticleRandomness), Random.Range(-ParticleRandomness, ParticleRandomness), Random.Range(-ParticleRandomness, ParticleRandomness));
        spawned.Velocity = Direction * ParticleSpeed + random;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawCube(WorldPos, Vector3.one * 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(WorldPos, WorldPos + Direction * 0.4f);
    }
}
