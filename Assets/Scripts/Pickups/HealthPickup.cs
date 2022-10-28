using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    public int health_;

    protected void PickUp(FirstPersonController player)
    {
        if (player.healthComponent_.Health >= player.healthComponent_.maxHealth_) return;
        player.healthComponent_.Heal(health_);
        UIManager.Instance.UpdateHealth();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PickUp(other.GetComponent<FirstPersonController>());
        }
    }
}
