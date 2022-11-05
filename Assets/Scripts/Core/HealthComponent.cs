using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [Header("Health Attributes")]
    public float maxHealth_;

    [SerializeField]
    private float health_;
    public float Health
    {
        get { return health_; }
    }

    private bool dead_;

    public delegate void OnHealthChangeDelegate();
    public event OnHealthChangeDelegate onTakeDamage_;
    public event OnHealthChangeDelegate onHeal_;
    public event OnHealthChangeDelegate onDeath_;

    public void TakeDamage(float damage)
    {
        if (dead_) return;
        health_ -= damage;
        if (health_ <= 0.0f)
        {
            Die();
        }
        else
        {
            if (onTakeDamage_ != null) onTakeDamage_();
        }
    }

    private void Awake()
    {
        health_ = maxHealth_;
        dead_ = false;
    }

    public void Heal(float health)
    {
        if (dead_) return;
        health_ += health;
        if (health_ > maxHealth_)
        {
            health_ = maxHealth_;
        }
        if (onHeal_ != null) onHeal_();
    }

    public void Die()
    {
        dead_ = true;
        if (onDeath_ != null) onDeath_();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer r in renderers)
        {
            r.enabled = false;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }

        Destroy(gameObject, 1.0f);
    }

}
