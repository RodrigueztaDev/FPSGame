using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Canvas Components")]
    public Text ammoText_;
    public Text ammoAmount_;
    public Text healthAmount_;
    public Image normalCrosshair_;
    public Image shotgunCrosshair_;

    public Text comboText_;
    public Slider comboProgressBar_;

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
        player_.weaponInventory_.onWeaponSwap_ += () => UpdateWeapon();
        player_.healthComponent_.onTakeDamage_ += () => UpdateHealth();
        player_.healthComponent_.onHeal_ += () => UpdateHealth();
        UpdateAmmo(player_.weaponInventory_.CurrentWeapon.TotalBulletAmmount);
        UpdateWeapon();
        UpdateHealth();
        SetComboValue(0.0f);
    }

    public void UpdateAmmo(float currentAmmo)
    {
        ammoAmount_.text = currentAmmo.ToString();
    }

    public void UpdateWeapon()
    {
        ammoText_.text = player_.weaponInventory_.CurrentWeapon.name;
        Weapon.WeaponType type = player_.weaponInventory_.CurrentWeapon.Type;
        if (type == Weapon.WeaponType.kShotgun || type == Weapon.WeaponType.kSuperShotgun)
        {
            shotgunCrosshair_.enabled = true;
            normalCrosshair_.enabled = false;
        }
        else
        {
            normalCrosshair_.enabled = true;
            shotgunCrosshair_.enabled = false;
        }
    }

    public void UpdateHealth()
    {
        healthAmount_.text = player_.healthComponent_.Health.ToString();
    }

    public void SetComboValue(float value)
    {
        comboProgressBar_.value = value;
    }

    public void AddComboValue(float value)
    {
        comboProgressBar_.value += value;
    }

    public float GetComboValue()
    {
        return comboProgressBar_.value;
    }

    public void ChangeComboText(ScoreManager.ScoreLevel scoreLevel)
    {
        switch (scoreLevel)
        {
            case ScoreManager.ScoreLevel.kF:
                {
                    comboText_.text = "F";
                    break;
                }
            case ScoreManager.ScoreLevel.kD:
                {
                    comboText_.text = "D";
                    break;
                }
            case ScoreManager.ScoreLevel.kC:
                {
                    comboText_.text = "C";
                    break;
                }
            case ScoreManager.ScoreLevel.kB:
                {
                    comboText_.text = "B";
                    break;

                }
            case ScoreManager.ScoreLevel.kA:
                {
                    comboText_.text = "A";
                    break;

                }
            case ScoreManager.ScoreLevel.kS:
                {
                    comboText_.text = "S";
                    break;

                }
            case ScoreManager.ScoreLevel.kSS:
                {
                    comboText_.text = "SS";
                    break;

                }
            case ScoreManager.ScoreLevel.kSSS:
                {
                    comboText_.text = "SSS";
                    break;

                }
        }
    }
}
