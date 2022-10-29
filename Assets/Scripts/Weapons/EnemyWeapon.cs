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
    public GameObject projectilePrefab_;
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
        CharacterController character = target.GetComponent<CharacterController>();

        Vector3 targetPosition = target.position;
        if (character != null)
        {
            targetPosition = new Vector3 (targetPosition.x, targetPosition.y + character.height * 0.5f, targetPosition.z);
        }

        Vector3 direction = (targetPosition - transform.position).normalized;
        Projectile b = Instantiate(projectilePrefab_, projectileSpawnRoot_.transform.position, Quaternion.identity).GetComponent<Projectile>();
        b.Damage = damage_;
        b.Shoot(direction);
        audioSource_.PlayOneShot(shotSound_, 0.2f);
        fireParticle_.Play();
        OnShoot();
    }
}
