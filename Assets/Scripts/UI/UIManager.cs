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
        Weapon currentWeapon = player_.currentWeapon_;
        currentWeapon.onReload_ += () => UpdateAmmo(currentWeapon.CurrentMagazineSize, currentWeapon.totalBulletAmount_);
        currentWeapon.onShoot_ += () => UpdateAmmo(currentWeapon.CurrentMagazineSize, currentWeapon.totalBulletAmount_);
        UpdateAmmo(currentWeapon.CurrentMagazineSize, currentWeapon.totalBulletAmount_);
    }

    public void UpdateAmmo(float currentAmmo, float totalAmmo)
    {
        ammoText_.text = currentAmmo.ToString() + " / " + totalAmmo.ToString();
    }
}
