using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    protected override void Awake()
    {
        base.Awake();
        type_ = WeaponType.kRifle;
    }

    protected override IEnumerator ShotUpdate()
    {
        currentShotAnimationTime_ = 0.0f;
        canSwapWeapon_ = false;
        canShoot_ = false;
        while (IsShooting())
        {
            currentShotAnimationTime_ += Time.deltaTime;
            transform.localPosition = new Vector3(0.0f, 0.0f, shotAnimationCurve_.Evaluate(currentShotAnimationTime_));
            yield return null;
        }
        transform.localPosition = Vector3.zero;
        canSwapWeapon_ = true;
        canShoot_ = true;
    }
}
