using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [Header("Health Attributes")]
    public float maxHealth_;

    private float health_;
    public float Health
    {
        get { return health_; }
    }

    public delegate void OnHealthChangeDelegate();
    public event OnHealthChangeDelegate onHealthChange_;

    public void TakeDamage(float damage)
    {
        health_ -= damage;
        if(onHealthChange_ != null) onHealthChange_();
        if (health_ <= 0.0f)
        {
            Die();
        }
    }

    private void Start()
    {
        health_ = maxHealth_;
    }

    public void Heal(float health)
    {
        health_ += health;
        if (health_ > maxHealth_)
        {
            health_ = maxHealth_;
        }
        if(onHealthChange_ != null) onHealthChange_();
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
