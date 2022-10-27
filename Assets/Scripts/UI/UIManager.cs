using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Canvas Components")]
    public Text ammoText_;

    public FirstPersonController player_;
    static private UIManager instance_;
    static public UIManager Instance
    {
        get { return instance_; }
    }

    void Start()
    {
        if (instance_ == null) instance_ = this;
        foreach (Weapon weapon in player_.weaponInventory_.Weapons)
        {
            weapon.onShowAnimation_ += () => UpdateAmmo(weapon.TotalBulletAmmount);
            weapon.onShoot_ += () => UpdateAmmo(weapon.TotalBulletAmmount);
        }
        UpdateAmmo(player_.weaponInventory_.CurrentWeapon.TotalBulletAmmount);
    }

    public void UpdateAmmo(float currentAmmo)
    {
        ammoText_.text = currentAmmo.ToString();
    }
}
