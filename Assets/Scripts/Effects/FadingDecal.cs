
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

public class FadingDecal : MonoBehaviour
{
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
    public DecalProjectorComponent Decal
    {
        get
        {
            if (_decal == null)
                _decal = GetComponent<DecalProjectorComponent>();
            return _decal;
        }
    }
    private DecalProjectorComponent _decal;

    public float FadeTime = 15f;
    public float Timer;
    public AnimationCurve FadeCurve;

    private void UponSpawn()
    {
        Timer = 0f;
    }

    private void Update()
    {
        Timer += Time.deltaTime;
        float p = Timer / FadeTime;
        float x = FadeCurve.Evaluate(p);

        Decal.fadeFactor = x;

        if (p >= 1f)
        {
            PoolObject.Despawn(this);
        }
    }
}
