using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Canvas Components")]
    public Text ammoText_;


    public FirstPersonController player_;

    void Start()
    {
        Weapon currentWeapon = player_.weaponInventory_.CurrentWeapon;
        currentWeapon.onShowAnimation_ += () => UpdateAmmo(currentWeapon.TotalBulletAmmount);
        currentWeapon.onShoot_ += () => UpdateAmmo(currentWeapon.TotalBulletAmmount);
        UpdateAmmo(currentWeapon.TotalBulletAmmount);
    }

    public void UpdateAmmo(float currentAmmo)
    {
        ammoText_.text = currentAmmo.ToString();
    }
}
