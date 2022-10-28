using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Bullet Attributes")]
    public float speed_;

    private float damage_;
    public float Damage
    {
        get { return damage_; }
        set { damage_ = value; }
    }

    public void Shoot(Vector3 direction)
    {
        GetComponent<Rigidbody>().AddForce(direction * speed_);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.tag == "Player")
        {
            obj.GetComponent<HealthComponent>().TakeDamage(damage_);
        }
    }
}
