using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string Name;
    public Pawn Pawn;

    public bool IsGun { get { return Gun != null; } }
    public Gun Gun
    {
        get
        {
            if (_gun == null)
                _gun = GetComponent<Gun>();
            return _gun;
        }
    }
    private Gun _gun;

    public bool IsMeeleWeapon { get { return MeeleWeapon != null; } }
    public MeeleWeapon MeeleWeapon
    {
        get
        {
            if (_meele == null)
                _meele = GetComponent<MeeleWeapon>();
            return _meele;
        }
    }
    private MeeleWeapon _meele;
}
