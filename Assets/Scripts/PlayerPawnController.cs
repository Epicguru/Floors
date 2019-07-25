using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawnController : MonoBehaviour
{
    public Pawn Pawn;
    public LayerMask FloorMask;

    private Vector3 forwards, right, dir;

    private void Update()
    {
        if (Pawn == null)
            return;

        UpdateMeele();
        UpdateGun();
        RotateCamera();
        SetRotationInput();
        SetDirectionInput();
    }

    private void UpdateGun()
    {
        Gun g = Pawn.Item?.Gun;

        if(g != null)
        {
            g.Shoot = Input.GetMouseButton(0);
            g.Reload = Input.GetKeyDown(KeyCode.R);

            if(g.MagBullets == 0 && g.Shoot && !g.IsReloading)
            {
                g.Reload = true;
            }
        }
    }

    private void UpdateMeele()
    {
        MeeleWeapon m = Pawn.Item?.MeeleWeapon;

        if(m != null)
        {
            m.LightAttack = Input.GetMouseButton(0);
            m.HeavyAttack = Input.GetMouseButton(1);
        }
    }

    private void RotateCamera()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            GameCamera.Instance.Angle -= Time.unscaledDeltaTime * 180f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            GameCamera.Instance.Angle += Time.unscaledDeltaTime * 180f;
        }
    }

    private void SetRotationInput()
    {
        Ray r = GameCamera.Instance.Camera.ScreenPointToRay(Input.mousePosition);

        bool didHit = Physics.Raycast(r, out RaycastHit hit, 1000f, FloorMask);

        if (didHit)
        {
            Vector3 point = hit.point;
            Vector2 hitFlat = new Vector2(point.x, point.z);
            Vector2 currentFlat = new Vector2(Pawn.transform.position.x, Pawn.transform.position.z);
            Vector2 diff = (hitFlat - currentFlat);
            float angle = 90f - diff.ToAngle();

            Pawn.Rotation = angle;
        }
    }

    private void SetDirectionInput()
    {
        // TODO make or use a proper input system.

        Vector2 rawInput = Vector2.zero;

        if (Input.GetKey(KeyCode.D))
            rawInput.x += 1f;
        if (Input.GetKey(KeyCode.A))
            rawInput.x -= 1f;
        if (Input.GetKey(KeyCode.W))
            rawInput.y += 1f;
        if (Input.GetKey(KeyCode.S))
            rawInput.y -= 1f;

        rawInput.Normalize();

        float forwardsAngle = GameCamera.Instance.Angle + 180f;

        forwards.x = Mathf.Cos(forwardsAngle * Mathf.Deg2Rad);
        forwards.z = Mathf.Sin(forwardsAngle * Mathf.Deg2Rad);
        forwards.y = 0f;

        right.x = Mathf.Cos((forwardsAngle - 90f) * Mathf.Deg2Rad);
        right.z = Mathf.Sin((forwardsAngle - 90f) * Mathf.Deg2Rad);
        right.y = 0f;

        dir = Vector3.zero;

        dir += forwards * rawInput.y;
        dir += right * rawInput.x;
        dir.Normalize();

        Pawn.DirectionInput = new Vector2(dir.x, dir.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + forwards);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + right);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + dir);
    }
}
