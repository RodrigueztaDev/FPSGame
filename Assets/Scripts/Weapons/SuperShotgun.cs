using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperShotgun : Shotgun
{
    protected override void Awake()
    {
        base.Awake();
        type_ = WeaponType.kSuperShotgun;
    }

    protected override IEnumerator AnimationUpdate()
    {
        currentShowAnimationTime_ = 0.0f;
        canSwapWeapon_ = false;
        canShoot_ = false;
        while (IsShowingAnimation())
        {
            currentShowAnimationTime_ += Time.deltaTime;
            transform.GetChild(0).localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, showAnimationCurve_.Evaluate(currentShowAnimationTime_)));
            yield return null;
        }

        transform.GetChild(0).localRotation = Quaternion.identity;
        canSwapWeapon_ = true;
        canShoot_ = true;
    }

    protected override IEnumerator ReloadUpdate()
    {
        currentReloadAnimationTime_ = 0.0f;
        canSwapWeapon_ = false;
        while (isReloading())
        {
            currentReloadAnimationTime_ += Time.deltaTime;
            transform.localPosition = new Vector3(0.0f, reloadAnimationCurve_.Evaluate(currentReloadAnimationTime_), 0.0f);
            yield return null;
        }
        transform.localRotation = Quaternion.identity;
        canSwapWeapon_ = true;
        canShoot_ = true;
    }
}
