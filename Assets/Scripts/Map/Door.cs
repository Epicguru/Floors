using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(GenericAnimationCallback))]
public class Door : MonoBehaviour
{
    public Animator Animator
    {
        get
        {
            if (_anim == null)
                _anim = GetComponent<Animator>();
            return _anim;
        }
    }
    private Animator _anim;

    public Vector3 CameraOffset = new Vector3(-3f, 7f, -10f);
    public Vector3 Center = new Vector3(0f, 1.5f, 0f);

    public bool IsOpening = false;
    public bool IsOpen = false;
    public bool CapturedCamera = false;

    public Vector3 CameraPos
    {
        get
        {
            return transform.TransformPoint(Center + CameraOffset);
        }
    }

    public Vector3 CenterPos
    {
        get
        {
            return transform.TransformPoint(Center);
        }
    }

    public Vector3 CameraAngles
    {
        get
        {
            return Quaternion.LookRotation(CenterPos - CameraPos).eulerAngles;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Open();
        }
    }

    public void Open(bool setCameraTarget = true)
    {
        if (IsOpen)
            return;

        if (setCameraTarget)
        {
            if (!IsOpening)
            {
                Vector3 pos = CameraPos;
                GameCamera cam = GameCamera.Instance;
                cam.OverridePos = pos;
                cam.OverrideAngle = CameraAngles;
                cam.Override = true;
                CapturedCamera = true;
                IsOpening = true;

                Invoke("TriggerAnim", 0.3f);
            }
            else
            {
                if(IsInvoking())
                    CancelInvoke("TriggerAnim");

                Animator.SetTrigger("Open");
                Animator.SetTrigger("Open");
            }
        }
        else
        {
            TriggerAnim();
            IsOpening = true;
        }
    }

    private void TriggerAnim()
    {
        Animator.SetTrigger("Open");
    }

    private void UponAnimationEvent(AnimationEvent e)
    {
        string s = e.stringParameter.Trim().ToLower();

        if(s == "open" || s == "opened")
        {
            IsOpen = true;
            IsOpening = false;
            if (CapturedCamera)
            {
                GameCamera.Instance.Override = false;
            }
        }
    }
}
