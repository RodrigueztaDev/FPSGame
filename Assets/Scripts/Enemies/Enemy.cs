using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Components")]
    public HealthComponent healthComponent_;
    public AudioSource audioSource_;

    [Header("Enemy Attributes")]
    public float materialChangeTime_ = 0.1f;
    public float attackCooldown = 1.0f;
    public EnemyWeapon weapon_;

    [Header("Sound")]
    public AudioClip[] damageSounds_;
    public AudioClip deathSound_;

    private Material material_;
    private Color materialDefaultColor_;

    // AI
    private NavMeshAgent agent_;
    private Transform target_;

    protected virtual void Start()
    {
        healthComponent_ = GetComponent<HealthComponent>();
        audioSource_ = GetComponent<AudioSource>();
        agent_ = GetComponent<NavMeshAgent>();
        target_ = GameObject.Find("PlayerCapsule").transform;
        healthComponent_.onTakeDamage_ += () => OnTakeDamage();
        healthComponent_.onDeath_ += () => PlayDeathSound();
        material_ = GetComponentInChildren<MeshRenderer>().material;
        materialDefaultColor_ = material_.color;
        InvokeRepeating("Shoot", attackCooldown, attackCooldown);
    }

    private void Shoot()
    {
        if(weapon_ != null)
        {
            weapon_.Shoot(target_);
        }
    }

    private void OnTakeDamage()
    {
        material_.color = Color.red;
        CancelInvoke();
        Invoke("DefaultMaterialColor", materialChangeTime_);
        audioSource_.PlayOneShot(RandomDamageSound(), 0.2f);
    }

    private void PlayDeathSound()
    {
        AudioManager.PlaySoundAtLocation(deathSound_, transform.position);
    }

    private AudioClip RandomDamageSound()
    {
        return damageSounds_[Random.Range(0, damageSounds_.Length)];
    }

    private void DefaultMaterialColor()
    {
        material_.color = materialDefaultColor_;
    }

    protected virtual void Update()
    {
        agent_.SetDestination(target_.transform.position);
    }
}
