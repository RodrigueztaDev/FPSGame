using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [Header("Sound")]
    public AudioSource audioSource_;
    public AudioClip shotSound_;


    [Header("Particle Effects")]
    public ParticleSystem fireParticle_;


    [Header("Weapon Attributes")]
    public GameObject projectileSpawnRoot_;
    public float damage_;

    public delegate void OnShootDelegate();
    public event OnShootDelegate onShoot_;

    protected Weapon.WeaponType type_;

    public Weapon.WeaponType Type
    {
        get { return type_; }
    }

    protected virtual void Awake()
    {
        Debug.Assert(projectileSpawnRoot_ != null);
        audioSource_ = gameObject.AddComponent<AudioSource>();

    }

    protected virtual void Update()
    {

    }

    protected void OnShoot()
    {
        if (onShoot_ != null) onShoot_();
    }

    public virtual void Shoot(Transform target)
    {
        RaycastHit hitInfo;

        Vector3 direction = (target.position - transform.position + Vector3.up).normalized;
        Physics.Raycast(transform.position, direction * 100.0f, out hitInfo);
        Debug.DrawRay(transform.position, direction * 100.0f, Color.red, 5.0f);
        if (hitInfo.collider != null)
        {
            GameObject obj = hitInfo.collider.gameObject;
            if (obj.tag == "Player")
            {
                obj.GetComponent<HealthComponent>().TakeDamage(damage_);
            }
        }
        audioSource_.PlayOneShot(shotSound_, 0.2f);
        fireParticle_.Play();
        OnShoot();
    }
}
