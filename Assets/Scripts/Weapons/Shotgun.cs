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
                RaycastHit hitInfo;
                float totalDamage = 0.0f;
                HealthComponent health = null;
                for (int i = 0; i < pelletNumber_; i++)
                {
                    Vector3 randomVector = new Vector3(Random.Range(-maxSpread_, maxSpread_), Random.Range(-maxSpread_, maxSpread_), Random.Range(-maxSpread_, maxSpread_));
                    Vector3 pelletDirection = (mainCamera.transform.forward + randomVector) * 100.0f;
                    Physics.Raycast(mainCamera.transform.position, pelletDirection, out hitInfo);
                    //Debug.DrawRay(mainCamera.transform.position, pelletDirection, Color.red, 5.0f);
                    if (hitInfo.collider != null)
                    {
                        GameObject obj = hitInfo.collider.gameObject;
                        switch (obj.tag)
                        {
                            case "EnemyHead":
                                {
                                    totalDamage += damage_ * headshotDamageMultiplier_;
                                    if (health == null) health = obj.transform.parent.GetComponent<HealthComponent>();
                                    break;
                                }
                            case "Enemy":
                                {
                                    totalDamage += damage_;
                                    if (health == null) health = obj.GetComponent<HealthComponent>();
                                    break;
                                }
                            case "Environment":
                                {
                                    Instantiate(bulletDecal_, hitInfo.point, Quaternion.LookRotation(pelletDirection));
                                    break;
                                }
                        }
                    }
                }

                if (health != null) health.TakeDamage(totalDamage);

                totalBulletAmount_--;
                audioSource_.PlayOneShot(shotSound_, 0.2f);
                fireParticle_.Play();
                OnShoot();
                isShootingAnimation_ = true;
                currentBulletCooldown_ = bulletCooldown_;
                currentshotAnimationTime_ = shotAnimationTime_;
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
            }
        }
    }
}
