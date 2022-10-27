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
                Camera mainCamera = Camera.main;
                RaycastHit hitInfo;
                for(int i = 0; i < pelletNumber_; i++)
                {
                    Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hitInfo);
                    Vector3 randomVector = new Vector3(Random.Range(-maxSpread_, maxSpread_), Random.Range(-maxSpread_, maxSpread_), Random.Range(-maxSpread_, maxSpread_));
                    Debug.DrawRay(mainCamera.transform.position, (mainCamera.transform.forward + randomVector) * 100.0f, Color.red, 5.0f);
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
