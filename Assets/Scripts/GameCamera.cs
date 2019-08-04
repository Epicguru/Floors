using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public static GameCamera Instance
    {
        get
        {
            if(_sCam == null)
            {
                _sCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameCamera>();
            }
            return _sCam;
        }
        private set
        {
            _sCam = value;
        }
    }
    private static GameCamera _sCam;

    public Camera Camera
    {
        get
        {
            if (_camera == null)
                _camera = GetComponent<Camera>();
            return _camera;
        }
    }
    private Camera _camera;

    public float Height = 5f;
    public Transform Target;
    public float Angle;

    public Vector3 OverridePos;
    public Vector3 OverrideAngle;

    public bool Override;
    public AnimationCurve LerpCurve;
    public float LerpTime = 1f;
    public float ZOffset = -10f;
    public float Tilt = 60f;

    private float overrideAmount = 0f;
    private Vector3 currentPos;
    private float lerpTimer;

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        if (Target == null)
            return;

        lerpTimer += Time.fixedDeltaTime * (Override ? 1f : -1f);
        lerpTimer = Mathf.Clamp(lerpTimer, 0f, LerpTime);
        float p = lerpTimer / LerpTime;
        float x = LerpCurve.Evaluate(p);
        overrideAmount = x;

        Vector3 posA = Target.transform.position;
        posA.y += Height;
        posA.z += ZOffset;
        Vector3 angleA = new Vector3(Tilt, Angle, 0f);
        Vector3 posB = OverridePos;
        Vector3 angleB = OverrideAngle;

        transform.position = Vector3.Lerp(posA, posB, overrideAmount);
        transform.localRotation = Quaternion.Lerp(Quaternion.Euler(angleA), Quaternion.Euler(angleB), overrideAmount);
    }
}
