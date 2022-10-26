using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    void Awake()
    {
        type_ = WeaponType.kPistol;
        audioSource_ = GetComponent<AudioSource>();
    }

    void Update()
    {
        ShotUpdate();
        ReloadUpdate();
    }

    public override void Shoot()
    {
        if(totalBulletAmount_ > 0 && currentBulletCooldown_ <= 0.0f)
        {
            if(!isShowingAnimation_)
            {
                GameObject bullet = Instantiate(projectilePrefab_,
                    projectileSpawnRoot_.transform.position, 
                    Quaternion.LookRotation(projectileSpawnRoot_.transform.forward, projectileSpawnRoot_.transform.up));
                Debug.Assert(bullet != null);
                bullet.GetComponent<Bullet>().Shoot(projectileSpawnRoot_.transform.forward, projectileSpeed_);
                totalBulletAmount_--;
                audioSource_.PlayOneShot(shotSound_);
                OnShoot();
                isShootingAnimation_ = true;
                currentBulletCooldown_ = bulletCooldown_;
            }
        }
        else
        {
            ShowAnimation();
        }
    }

    public override void ShowAnimation()
    {
        if(!isShowingAnimation_ && !isShootingAnimation_)
        {
            showAnimationTime_ = 0.0f;
            isShowingAnimation_ = true;
        }
    }

    protected override void ReloadUpdate()
    {
        if(isShowingAnimation_)
        {
            showAnimationTime_ += Time.deltaTime;
            transform.localRotation = Quaternion.Euler(new Vector3(showAnimationCurve_.Evaluate(showAnimationTime_), 0f, 0f));
            if (showAnimationTime_ >= totalAnimationTime_)
            {
                transform.localRotation = Quaternion.identity;
                isShowingAnimation_ = false;
                OnShowAnimation();
            }
        }
    }

    protected override void ShotUpdate()
    {
        if (isShootingAnimation_)
        {
            currentBulletCooldown_ -= Time.deltaTime;
            transform.localRotation = Quaternion.Euler(new Vector3(shotAnimationCurve_.Evaluate(currentBulletCooldown_), 0f, 0f));
            if (currentBulletCooldown_ <= 0.0f)
            {
                transform.localRotation = Quaternion.identity;
                isShootingAnimation_ = false;
            }
        }
    }
}
