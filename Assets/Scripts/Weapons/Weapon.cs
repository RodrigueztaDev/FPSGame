using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Sound")]
    public AudioClip shotSound_;


    [Header("Animation")]
    public AnimationCurve showAnimationCurve_;
    public AnimationCurve shotAnimationCurve_;

    [Header("Weapon Attributes")]
    public GameObject projectileSpawnRoot_;
    public float totalAnimationTime_;
    public float bulletCooldown_;
    [SerializeField]
    protected int totalBulletAmount_;
    public int TotalBulletAmmount { get { return totalBulletAmount_; } }

    public delegate void OnShootDelegate();
    public event OnShootDelegate onShoot_;

    public delegate void OnShowAnimationDelegate();
    public event OnShowAnimationDelegate onShowAnimation_;
    public enum WeaponType
    {
        kPistol,
        kRifle,
        kShotgun,
        kSuperShotgun,
        kCrossbow,
        kMissileLauncher,
    }

    protected WeaponType type_;

    public WeaponType Type
    {
        get { return type_; }
    }

    protected bool isShowingAnimation_;
    protected float showAnimationTime_;
    protected bool isShootingAnimation_;
    protected float currentBulletCooldown_;

    private void Awake()
    {
        Debug.Assert(projectileSpawnRoot_ != null);
    }

    private void Update()
    {
        
    }

    protected void OnShoot()
    {
        if(onShoot_ != null) onShoot_();
    }

    protected void OnShowAnimation()
    {
        if(onShowAnimation_ != null) onShowAnimation_();
    }

    public virtual void Shoot()
    {
        if (totalBulletAmount_ > 0 && currentBulletCooldown_ <= 0.0f)
        {
            if (!isShowingAnimation_)
            {
                Camera mainCamera = Camera.main;
                RaycastHit hitInfo;
                Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hitInfo);
                Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 100.0f, Color.red, 5.0f);
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

    public void AddAmmo(int ammoToAdd)
    {
        totalBulletAmount_ += ammoToAdd;
    }

    public virtual void ShowAnimation()
    {
        if (!isShowingAnimation_ && !isShootingAnimation_)
        {
            showAnimationTime_ = 0.0f;
            isShowingAnimation_ = true;
        }
    }

    protected virtual void AnimationUpdate()
    {
        if (isShowingAnimation_)
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

    protected virtual void ShotUpdate()
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