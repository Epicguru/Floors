
using UnityEngine;

public class Spawnables : MonoBehaviour
{
    public static BloodParticle BloodParticle { get { return Instance._bloodParticle; } }
    public BloodParticle _bloodParticle;
    public static PoolObject SparksEffect { get { return Instance._sparksEffect; } }
    public PoolObject _sparksEffect;

    private static Spawnables Instance
    {
        get
        {
            if(_ins == null)
            {
                _ins = GameObject.FindGameObjectWithTag("SpawnablesManager").GetComponent<Spawnables>();
            }
            return _ins;
        }
    }
    private static Spawnables _ins;

    private void Awake()
    {
        _ins = this;
    }
}