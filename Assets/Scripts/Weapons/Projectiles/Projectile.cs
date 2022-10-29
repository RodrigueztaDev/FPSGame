using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Bullet Attributes")]
    public float speed_;

    protected float damage_;
    public float Damage
    {
        get { return damage_; }
        set { damage_ = value; }
    }

    public void Shoot(Vector3 direction)
    {
        GetComponent<Rigidbody>().AddForce(direction * speed_);
    }

    protected virtual void OnCollition(GameObject o)
    {
        if (o.tag == "Player")
        {
            o.GetComponent<HealthComponent>().TakeDamage(damage_);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnCollition(other.gameObject);
    }
}
