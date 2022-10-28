using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public enum EnemyState
    {
        kIdle,
        kShooting,
        kMoving,
        kShootingAndMoving,
    }
    public EnemyState state_;

    [Header("Enemy Components")]
    public HealthComponent healthComponent_;
    public AudioSource audioSource_;
    public GameObject damageParticle_;

    private ParticleSystem[] damageParticlePool_;

    [Header("Enemy Attributes")]
    public float materialChangeTime_ = 0.1f;
    public float attackCooldown = 1.0f;
    public int particlePoolLenght_;
    public EnemyWeapon weapon_;

    [Header("Sound")]
    public AudioClip[] damageSounds_;
    public AudioClip deathSound_;

    private Material material_;
    private Color materialDefaultColor_;

    [Header("AI")]
    public float detectionRange_;
    public float directionChangeChance_ = 60.0f;
    private float distanceToPlayer_;
    private Transform player_;
    private Vector3 target_;
    private NavMeshAgent agent_;

    protected virtual void Start()
    {
        healthComponent_ = GetComponent<HealthComponent>();
        audioSource_ = GetComponent<AudioSource>();
        agent_ = GetComponent<NavMeshAgent>();
        state_ = EnemyState.kIdle;
        player_ = GameObject.Find("PlayerCapsule").transform;
        healthComponent_.onTakeDamage_ += () => OnTakeDamage();
        healthComponent_.onDeath_ += () => PlayDeathSound();
        material_ = GetComponentInChildren<MeshRenderer>().material;
        materialDefaultColor_ = material_.color;

        damageParticlePool_ = new ParticleSystem[particlePoolLenght_];
        for (int i = 0; i < particlePoolLenght_; ++i)
        {
            GameObject particle = Instantiate(damageParticle_, transform.position, Quaternion.identity);
            damageParticlePool_[i] = particle.GetComponent<ParticleSystem>();
            particle.transform.parent = transform;
        }
    }

    protected virtual void CheckState()
    {
        switch (state_)
        {
            case EnemyState.kIdle:
                {
                    distanceToPlayer_ = Vector3.Distance(player_.position, transform.position);
                    if(distanceToPlayer_ <= detectionRange_)
                    {
                        target_ = player_.position + (Random.insideUnitSphere * (detectionRange_ * 0.5f));
                        state_ = EnemyState.kMoving;
                        agent_.SetDestination(target_);
                    }
                    break;
                }
            case EnemyState.kMoving:
                {
                    if(agent_.remainingDistance < float.Epsilon)
                    {
                        distanceToPlayer_ = Vector3.Distance(player_.position, transform.position);
                        state_ = EnemyState.kShootingAndMoving;
                        InvokeRepeating("Shoot", attackCooldown, attackCooldown);
                    }
                    break;
                }
            case EnemyState.kShootingAndMoving:
                {
                    Vector3 forward = player_.position - transform.position;
                    transform.rotation = Quaternion.LookRotation(forward);

                    float rand = Random.Range(0.0f, 100.0f);
                    if(rand <= directionChangeChance_)
                    {
                        target_ = player_.position + (Random.insideUnitSphere * (detectionRange_ * 0.5f));
                        agent_.SetDestination(target_);
                    }
                    break;
                }
        }
    }


    private void Shoot()
    {
        if(weapon_ != null)
        {
            weapon_.Shoot(player_);
        }
    }

    private ParticleSystem GetNextParticleFromPool()
    {
        foreach(var particle in damageParticlePool_)
        {
            if (!particle.isPlaying) return particle;
        }
        return null;
    }

    private void OnTakeDamage()
    {
        GetNextParticleFromPool().Play();
        material_.color = Color.red;
        CancelInvoke("DefaultMaterialColor");
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
        CheckState();
    }
}
