using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    void Awake()
    {
        type_ = WeaponType.kPistol;
    }

    void Update()
    {
        ShotUpdate();
        AnimationUpdate();
    }
}
