using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

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
    public float Distance = 6f;
    public Transform Target;
    public float Angle = 45f;

    private Vector3 currentPos;

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        if (Target == null)
            return;

        Vector3 pos = Target.transform.position;
        pos.y += Height;

        float x = Mathf.Cos(Angle * Mathf.Deg2Rad) * Distance;
        float z = Mathf.Sin(Angle * Mathf.Deg2Rad) * Distance;

        pos.x += x;
        pos.z += z;

        transform.position = pos;
        transform.LookAt(Target);
    }
}
