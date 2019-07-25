using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolObject))]
public class AutoDestroy : MonoBehaviour
{
    public ParticleSystem Particles;

    public PoolObject PoolObject
    {
        get
        {
            if (_po == null)
                _po = GetComponent<PoolObject>();
            return _po;
        }
    }
    private PoolObject _po;

    public float Time = 1f;

    private void UponSpawn()
    {
        if(Particles != null)
            Particles.Play();
        Invoke("Kill", Time);
    }

    private void UponDespawn()
    {
        if(Particles != null)
            Particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void Kill()
    {
        PoolObject.Despawn(PoolObject);
    }
}
