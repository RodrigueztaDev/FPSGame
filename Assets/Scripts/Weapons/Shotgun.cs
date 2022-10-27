using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{

    [Header("Shotgun Attributes")]
    public int pelletNumber_;
    public float maxSpread_;

    void Awake()
    {
        type_ = WeaponType.kShotgun;
    }

    void Update()
    {
        ShotUpdate();
        AnimationUpdate();
    }

    public override void Shoot()
    {
        if (totalBulletAmount_ > 0 && currentBulletCooldown_ <= 0.0f)
        {
            if (!isShowingAnimation_)
            {
                for(int i = 0; i < pelletNumber_; i++)
                {
                    GameObject bullet = Instantiate(projectilePrefab_,
                        projectileSpawnRoot_.transform.position,
                        Quaternion.LookRotation(projectileSpawnRoot_.transform.forward, projectileSpawnRoot_.transform.up));
                    Debug.Assert(bullet != null);
                    Vector3 randomVector = new Vector3(Random.Range(-maxSpread_, maxSpread_), Random.Range(-maxSpread_, maxSpread_), Random.Range(-maxSpread_, maxSpread_));
                    bullet.GetComponent<Bullet>().Shoot(projectileSpawnRoot_.transform.forward + randomVector, projectileSpeed_);
                }

                totalBulletAmount_--;
                AudioManager.PlaySoundAtLocation(shotSound_, transform.position, 0.2f);
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
