using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    void Awake()
    {
        type_ = WeaponType.kRifle;
    }

    void Update()
    {
        ShotUpdate();
        AnimationUpdate();
    }

    protected override void ShotUpdate()
    {
        if (isShootingAnimation_)
        {
            currentBulletCooldown_ -= Time.deltaTime;
            transform.localPosition = new Vector3(0.0f, 0.0f, shotAnimationCurve_.Evaluate(currentBulletCooldown_));
            if (currentBulletCooldown_ <= 0.0f)
            {
                transform.localRotation = Quaternion.identity;
                isShootingAnimation_ = false;
            }
        }
    }
}
