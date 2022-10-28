using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRifle : EnemyWeapon
{
    protected override void Awake()
    {
        base.Awake();
        type_ = Weapon.WeaponType.kRifle;
    }
}
