using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{

    [Header("Shotgun Attributes")]
    public int pelletNumber_;
    public float maxSpread_;

    [Header("Shotgun Animation")]
    public AnimationCurve reloadAnimationCurve_;
    public float reloadAnimationTime_;
    public float shotAnimationTime_;

    private float currentshotAnimationTime_;
    private float currentReloadAnimationTime_;
    private bool isReloadingAnimation_;

    protected override void Awake()
    {
        base.Awake();
        type_ = WeaponType.kShotgun;
    }

    protected override void Update()
    {
        base.Update();
        ShotUpdate();
        ReloadUpdate();
    }

    public override void Shoot()
    {
        if (totalBulletAmount_ > 0 && currentBulletCooldown_ <= 0.0f)
        {
            if (!isShowingAnimation_ && !isReloadingAnimation_)
            {
                Camera mainCamera = Camera.main;
                RaycastHit hit;
                for (int i = 0; i < pelletNumber_; i++)
                {
                    Vector3 randomVector = new Vector3(Random.Range(-maxSpread_, maxSpread_), Random.Range(-maxSpread_, maxSpread_), Random.Range(-maxSpread_, maxSpread_));
                    Vector3 pelletDirection = (mainCamera.transform.forward + randomVector) * 100.0f;

                    TrailRenderer trail = Instantiate(bulletTrailRenderer_, projectileSpawnRoot_.transform.position, Quaternion.identity).GetComponent<TrailRenderer>();
                    if (Physics.Raycast(mainCamera.transform.position, pelletDirection, out hit))
                    {
                        StartCoroutine(UpdateTrail(trail, hit.point, hit.normal, hit.collider.gameObject));
                    }
                    else
                    {
                        StartCoroutine(UpdateTrail(trail,
                            projectileSpawnRoot_.transform.position + pelletDirection * 100.0f,
                            Vector3.zero,
                            null));
                    }
                }
                totalBulletAmount_--;
                audioSource_.PlayOneShot(shotSound_, 0.2f);
                fireParticle_.Play();
                OnShoot();
                isShootingAnimation_ = true;
                currentBulletCooldown_ = bulletCooldown_;
                currentshotAnimationTime_ = shotAnimationTime_;
                canSwapWeapon_ = false;
            }
        }
        else
        {
            ShowAnimation();
        }
    }

    public override void ShowAnimation()
    {
        if (!isShowingAnimation_ && !isShootingAnimation_ && !isReloadingAnimation_)
        {
            showAnimationTime_ = 0.0f;
            isShowingAnimation_ = true;
            canSwapWeapon_ = false;
        }
    }

    protected override void ShotUpdate()
    {
        if (isShootingAnimation_)
        {
            currentBulletCooldown_ -= Time.deltaTime;
            currentshotAnimationTime_ -= Time.deltaTime;
            transform.localPosition = new Vector3(0.0f, 0.0f, shotAnimationCurve_.Evaluate(currentshotAnimationTime_));
            if (currentshotAnimationTime_ <= 0.0f)
            {
                transform.localRotation = Quaternion.identity;
                isShootingAnimation_ = false;
                isReloadingAnimation_ = true;
                currentReloadAnimationTime_ = reloadAnimationTime_;
            }
        }
    }

    protected void ReloadUpdate()
    {
        if (isReloadingAnimation_)
        {
            currentBulletCooldown_ -= Time.deltaTime;
            currentReloadAnimationTime_ -= Time.deltaTime;
            transform.localRotation = Quaternion.Euler(new Vector3(reloadAnimationCurve_.Evaluate(currentReloadAnimationTime_), 0f, 0f));
            if (currentReloadAnimationTime_ <= 0.0f)
            {
                transform.localRotation = Quaternion.identity;
                isReloadingAnimation_ = false;
                canSwapWeapon_ = true;
            }
        }
    }
}
