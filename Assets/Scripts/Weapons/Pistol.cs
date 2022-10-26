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
        ReloadUpdate();
    }

    public override void Shoot()
    {
        if(totalBulletAmount_ > 0)
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
            }
        }
        else
        {
            ShowAnimation();
        }
    }

    public override void ShowAnimation()
    {
        if(!isShowingAnimation_)
        {
            animationTime_ = 0.0f;
            isShowingAnimation_ = true;
        }
    }

    protected override void ReloadUpdate()
    {
        if(isShowingAnimation_)
        {
            animationTime_ += Time.deltaTime;
            transform.localRotation = Quaternion.Euler(new Vector3(animationCurve_.Evaluate(animationTime_), 0f, 0f));
            if (animationTime_ >= totalAnimationTime_)
            {
                transform.localRotation = Quaternion.identity;
                isShowingAnimation_ = false;
                OnShowAnimation();
            }
        }
    }
}
