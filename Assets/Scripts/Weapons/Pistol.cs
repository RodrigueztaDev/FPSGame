using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    protected override void Awake()
    {
        base.Awake();
        type_ = WeaponType.kPistol;
    }
}
