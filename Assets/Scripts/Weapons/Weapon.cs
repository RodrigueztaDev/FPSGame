using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Sound")]
    public AudioSource audioSource_;
    public AudioClip shotSound_;


    [Header("Animation")]
    public AnimationCurve showAnimationCurve_;
    public AnimationCurve shotAnimationCurve_;

    [Header("Particle Effects")]
    public ParticleSystem fireParticle_;
    public GameObject bulletTrailRenderer_;

    [Header("Weapon Attributes")]
    public GameObject projectileSpawnRoot_;
    public float damage_;
    public float bulletSpeed_ = 100.0f;
    public float headshotDamageMultiplier_ = 1.5f;
    public GameObject bulletDecal_;
    [SerializeField]
    protected int maxBulletAmount_;
    protected int totalBulletAmount_;
    protected bool canSwapWeapon_;
    public bool CanSwapWeapon
    {
        get { return canSwapWeapon_; }
    }
    public int MaxBulletAmount { get { return maxBulletAmount_; } }
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

    private float showAnimationTime_;
    private float shotAnimationTime_;
    protected float currentShowAnimationTime_;
    protected float currentShotAnimationTime_;
    protected bool canShoot_;

    protected virtual void Awake()
    {
        Debug.Assert(projectileSpawnRoot_ != null);
        audioSource_ = gameObject.AddComponent<AudioSource>();

        totalBulletAmount_ = maxBulletAmount_;
        canSwapWeapon_ = true;
        showAnimationTime_ = showAnimationCurve_.keys[showAnimationCurve_.keys.Length - 1].time;
        shotAnimationTime_ = shotAnimationCurve_.keys[showAnimationCurve_.keys.Length - 1].time;
        currentShowAnimationTime_ = showAnimationTime_;
        currentShotAnimationTime_ = shotAnimationTime_;

        canShoot_ = true;
    }

    protected virtual void Update()
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

    protected IEnumerator UpdateTrail(TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal, GameObject hitObject/*, bool madeImpact*/)
    {
        Vector3 startPosition = trail.transform.position;
        float distance = Vector3.Distance(trail.transform.position, hitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0.0f)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1.0f - (remainingDistance / distance));
            remainingDistance -= bulletSpeed_ * Time.deltaTime;

            yield return null;
        }
        trail.transform.position = hitPoint;

        if(hitObject != null)
        {
            Camera mainCamera = Camera.main;
            switch (hitObject.tag)
        {
            case "EnemyHead":
                {
                    hitObject.transform.parent.GetComponent<HealthComponent>().TakeDamage(damage_ * headshotDamageMultiplier_);
                    break;
                }
            case "Enemy":
                {
                    hitObject.GetComponent<HealthComponent>().TakeDamage(damage_);
                    break;
                }
            case "Environment":
                {
                    Instantiate(bulletDecal_, hitPoint, Quaternion.LookRotation(hitNormal));
                    break;
                }
        }
        }

        Destroy(trail.gameObject, trail.time);
    }

    public virtual void Shoot()
    {
        if (totalBulletAmount_ > 0)
        {
            if (canShoot_)
            {
                Camera mainCamera = Camera.main;
                TrailRenderer trail = Instantiate(bulletTrailRenderer_, projectileSpawnRoot_.transform.position, Quaternion.identity).GetComponent<TrailRenderer>();
                RaycastHit hit;
                if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
                {
                    StartCoroutine(UpdateTrail(trail, hit.point, hit.normal, hit.collider.gameObject));
                }
                else
                {
                    StartCoroutine(UpdateTrail(trail, 
                        projectileSpawnRoot_.transform.position + mainCamera.transform.forward * 100.0f, 
                        Vector3.zero, 
                        null));
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

    public void AddAmmo(int ammoToAdd)
    {
        totalBulletAmount_ += ammoToAdd;
        if (totalBulletAmount_ > maxBulletAmount_) totalBulletAmount_ = maxBulletAmount_;
    }

    public virtual void ShowAnimation()
    {
        if (!IsShooting() && !IsShowingAnimation())
        {
            StartCoroutine("AnimationUpdate");
            OnShowAnimation();
        }
    }

    protected bool IsShowingAnimation()
    {
        return currentShowAnimationTime_ < showAnimationTime_;
    }

    protected bool IsShooting()
    {
        return currentShotAnimationTime_ < shotAnimationTime_;
    }

    protected virtual IEnumerator AnimationUpdate()
    {
        currentShowAnimationTime_ = 0.0f;
        canSwapWeapon_ = false;
        canShoot_ = false;
        while(IsShowingAnimation())
        {
            currentShowAnimationTime_ += Time.deltaTime;
            transform.localRotation = Quaternion.Euler(new Vector3(showAnimationCurve_.Evaluate(currentShowAnimationTime_), 0f, 0f));
            yield return null;
        }

        transform.localRotation = Quaternion.identity;
        canSwapWeapon_ = true;
        canShoot_ = true;
    }

    protected virtual IEnumerator ShotUpdate()
    {
        currentShotAnimationTime_ = 0.0f;
        canSwapWeapon_ = false;
        canShoot_ = false;
        while (IsShooting())
        {
            currentShotAnimationTime_ += Time.deltaTime;
            transform.localRotation = Quaternion.Euler(new Vector3(shotAnimationCurve_.Evaluate(currentShotAnimationTime_), 0f, 0f));
            yield return null;
        }
        transform.localRotation = Quaternion.identity;
        canSwapWeapon_ = true;
        canShoot_ = true;
    }
}