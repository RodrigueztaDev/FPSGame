using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    void Awake()
    {
        type_ = WeaponType.kPistol;
        currentMagazineSize_ = magazineMaxSize_;
    }

    void Update()
    {
        ReloadUpdate();
    }

    public override void Shoot()
    {
        if(currentMagazineSize_ > 0)
        {
            GameObject bullet = Instantiate(projectilePrefab_,
                projectileSpawnRoot_.transform.position, 
                Quaternion.LookRotation(projectileSpawnRoot_.transform.forward, projectileSpawnRoot_.transform.up));
            Debug.Assert(bullet != null);
            bullet.GetComponent<Bullet>().Shoot(projectileSpawnRoot_.transform.forward, projectileSpeed_);
            currentMagazineSize_--;
            OnShoot();
        }
        else
        {
            Reload();
        }
    }

    public override void Reload()
    {
        if(!isReloading_ && totalBulletAmount_ > 0)
        {
            reloadingTime_ = 0.0f;
            isReloading_ = true;
        }
    }

    protected override void ReloadUpdate()
    {
        if(isReloading_)
        {
            reloadingTime_ += Time.deltaTime;
            transform.localPosition = new Vector3(transform.localPosition.x, reloadAnimationCurve_.Evaluate(reloadingTime_), transform.localPosition.z);
            if (reloadingTime_ >= reloadTime_)
            {
                isReloading_ = false;
                totalBulletAmount_ -= magazineMaxSize_ - currentMagazineSize_;
                if(totalBulletAmount_ < 0)
                {
                    currentMagazineSize_ = magazineMaxSize_ + totalBulletAmount_;
                    totalBulletAmount_ = 0;
                }
                else
                {
                    currentMagazineSize_ = magazineMaxSize_;
                }
                OnReload();
            }
        }
    }
}
