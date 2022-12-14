using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Pickup
{
    public Weapon.WeaponType type_; // Type of the ammo to be added
    public int amountToBeAdded_;

    protected void PickUp(FirstPersonController player)
    {
        Weapon weapon = player.weaponInventory_.GetWeaponOfType(type_);
        if (weapon.TotalBulletAmmount >= weapon.MaxBulletAmount) return;
        weapon.AddAmmo(amountToBeAdded_);
        if(weapon == player.weaponInventory_.CurrentWeapon) UIManager.Instance.UpdateAmmo(weapon.TotalBulletAmmount);
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
