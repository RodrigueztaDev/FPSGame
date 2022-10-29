using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : Weapon
{

    public float maxBoltDistance_;
    public Transform visualProjectile_;
    public LayerMask objectsToHit_;

    private RaycastHit[] hitObjects_;

    protected override void Awake()
    {
        base.Awake();
        type_ = WeaponType.kCrossbow;
    }

    protected IEnumerator UpdateTrail(TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal, GameObject hitObject, int nextIndex)
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

        if (hitObject != null)
        {
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
                        //Instantiate(bulletDecal_, hitPoint, Quaternion.LookRotation(hitNormal));
                        Destroy(trail.gameObject, trail.time);
                        yield break;
                    }
            }
        }

        if (nextIndex < hitObjects_.Length) { 
            StartCoroutine(UpdateTrail(trail, hitObjects_[nextIndex].point, hitObjects_[nextIndex].normal, hitObjects_[nextIndex].collider.gameObject, nextIndex + 1));
        }
        else
        {
            Destroy(trail.gameObject, trail.time);
        }
    }

    public override void Shoot()
    {
        if (totalBulletAmount_ > 0)
        {
            if (canShoot_)
            {
                Camera mainCamera = Camera.main;
                TrailRenderer trail = Instantiate(bulletTrailRenderer_, projectileSpawnRoot_.transform.position, Quaternion.identity).GetComponent<TrailRenderer>();
                Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
                hitObjects_ = Physics.RaycastAll(ray, maxBoltDistance_, objectsToHit_);

                System.Array.Sort(hitObjects_, (x, y) => x.distance.CompareTo(y.distance)); // Sort raycast hits from closer to farther from player

                if (hitObjects_.Length > 0)
                {
                    StartCoroutine(UpdateTrail(trail, hitObjects_[0].point, hitObjects_[0].normal, hitObjects_[0].collider.gameObject, 1));
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
                if (fireParticle_ != null) fireParticle_.Play();
                OnShoot();
                StartCoroutine(ShotUpdate());
            }
        }
        else
        {
            ShowAnimation();
        }
    }

    protected override IEnumerator ShotUpdate()
    {
        visualProjectile_.gameObject.SetActive(false);
        currentShotAnimationTime_ = 0.0f;
        canSwapWeapon_ = false;
        canShoot_ = false;
        while (IsShooting())
        {
            currentShotAnimationTime_ += Time.deltaTime;

            if(currentShotAnimationTime_ >= shotAnimationTime_ * 0.5f)
            {
                visualProjectile_.gameObject.SetActive(true);
            }
            visualProjectile_.localPosition = new Vector3(0.0f, shotAnimationCurve_.Evaluate(currentShotAnimationTime_), 0.0f);
            yield return null;
        }
        transform.localPosition = Vector3.zero;
        canSwapWeapon_ = true;
        canShoot_ = true;
    }

    protected override IEnumerator AnimationUpdate()
    {
        currentShowAnimationTime_ = 0.0f;
        canSwapWeapon_ = false;
        canShoot_ = false;
        while (IsShowingAnimation())
        {
            currentShowAnimationTime_ += Time.deltaTime;
            visualProjectile_.localRotation = Quaternion.Euler(new Vector3(0.0f, showAnimationCurve_.Evaluate(currentShowAnimationTime_), 0.0f));
            yield return null;
        }

        visualProjectile_.localRotation = Quaternion.identity;
        canSwapWeapon_ = true;
        canShoot_ = true;
    }

}
