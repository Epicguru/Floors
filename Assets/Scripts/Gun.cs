﻿using UnityEngine;

[RequireComponent(typeof(Item))]
public class Gun : MonoBehaviour
{
    public Item Item
    {
        get
        {
            if (_item == null)
                _item = GetComponent<Item>();
            return _item;
        }
    }
    private Item _item;
    public Animator Animator
    {
        get
        {
            if (_anim == null)
                _anim = GetComponentInChildren<Animator>();
            return _anim;
        }
    }
    private Animator _anim;

    [Header("Shooting")]
    public Projectile Bullet;
    public Transform Muzzle;

    [Header("Stats")]
    public int MagCapacity = 30;
    public int MagBullets = 30;
    public float RPM = 650f;

    [Header("Input")]
    public bool Safe = false;
    public bool Shoot = false;
    public bool Reload = false;

    [Header("State")]
    public bool IsReloading;

    public float MinShotInterval
    {
        get
        {
            return 1f / (RPM / 60f);
        }
    }

    private float shotTimer;

    private void Update()
    {
        shotTimer -= Time.deltaTime;
        if (shotTimer < 0f)
            shotTimer = 0f;

        Animator.SetBool("Safe", Safe);

        if(!Safe && !IsReloading && shotTimer == 0f && MagBullets > 0)
        {
            if (Shoot)
            {
                shotTimer = MinShotInterval;
                Animator.SetTrigger("Shoot");
            }
        }

        if(!Safe && !IsReloading)
        {
            if (Reload)
            {
                Reload = false;
                Animator.SetTrigger("Reload");
                IsReloading = true;
            }
        }
    }

    private void SpawnBullet()
    {
        Projectile p = PoolObject.Spawn(Bullet);

        p.Direction = Item.Pawn.transform.forward;

        //p.transform.position = Muzzle.transform.position;
        p.transform.position = Item.Pawn.transform.position + p.Direction * 1f + Vector3.up * 1f;
        p.transform.forward = p.Direction;

    }

    public void UponAnimationEvent(AnimationEvent e)
    {
        string s = e.stringParameter.Trim().ToLower();

        switch (s)
        {
            case "shoot":

                if (IsReloading)
                    return;

                if(MagBullets <= 0)
                {
                    Debug.LogWarning("Shoot anim when no bullet in magazine!");
                    break;
                }

                MagBullets--;

                SpawnBullet();

                break;

            case "reload":

                MagBullets = MagCapacity;
                IsReloading = false;

                break;
        }
    }
}