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

    protected float reloadAnimationTime_;
    protected float currentReloadAnimationTime_;

    protected override void Awake()
    {
        base.Awake();
        type_ = WeaponType.kShotgun;

        reloadAnimationTime_ = reloadAnimationCurve_.keys[reloadAnimationCurve_.keys.Length - 1].time;
        currentReloadAnimationTime_ = reloadAnimationTime_;
    }

    public override void Shoot()
    {
        if (totalBulletAmount_ > 0)
        {
            if (canShoot_)
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
                StartCoroutine("ShotUpdate");
            }
        }
        else
        {
            ShowAnimation();
        }
    }

    protected bool isReloading()
    {
        return currentReloadAnimationTime_ < reloadAnimationTime_;
    }

    public override void ShowAnimation()
    {
        if (!IsShowingAnimation() && !IsShooting() && !isReloading())
        {
            StartCoroutine("AnimationUpdate");
            OnShowAnimation();
        }
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
        StartCoroutine("ReloadUpdate");
    }

    protected virtual IEnumerator ReloadUpdate()
    {
        currentReloadAnimationTime_ = 0.0f;
        canSwapWeapon_ = false;
        while (isReloading())
        { 
            currentReloadAnimationTime_ += Time.deltaTime;
            transform.localRotation = Quaternion.Euler(new Vector3(reloadAnimationCurve_.Evaluate(currentReloadAnimationTime_), 0f, 0f));
            yield return null;
        }
        transform.localRotation = Quaternion.identity;
        canSwapWeapon_ = true;
        canShoot_ = true;
    }
}
