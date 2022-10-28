using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    public List<GameObject> weaponPrefabs_;
    private Weapon[] weapons_; // Using an array instead of a dictionary to have elements in order
    public Weapon[] Weapons
    {
        get { return weapons_; }
    }
    public Weapon CurrentWeapon
    {
        get { return weapons_[currentWeaponIndex_]; }
    }
    private int currentWeaponIndex_;

    public delegate void OnSwapWeaponDelegate();
    public event OnSwapWeaponDelegate onWeaponSwap_;

    void Awake()
    {
        weapons_ = new Weapon[weaponPrefabs_.Count];
        Transform weaponRootTransform = GameObject.Find("PlayerWeaponRoot").transform;
        for(int i = 0; i < weapons_.Length; i++)
        {
            weapons_[i] = Instantiate(weaponPrefabs_[i], weaponRootTransform.position, Quaternion.identity).GetComponent<Weapon>();
            weapons_[i].transform.parent = weaponRootTransform;
            weapons_[i].gameObject.SetActive(false);
            weapons_[i].name = weapons_[i].name.Remove(weapons_[i].name.Length - 7); // Remove "(Clone)" from weapon name
        }
        currentWeaponIndex_ = 0;
        weapons_[currentWeaponIndex_].gameObject.SetActive(true);
    }

    public void SwapToNextWeapon()
    {
        if (currentWeaponIndex_ + 1 >= weapons_.Length) return;

        weapons_[currentWeaponIndex_].gameObject.SetActive(false);
        currentWeaponIndex_++;
        weapons_[currentWeaponIndex_].gameObject.SetActive(true);
        UIManager.Instance.UpdateAmmo(weapons_[currentWeaponIndex_].TotalBulletAmmount);
        if(onWeaponSwap_ != null) onWeaponSwap_();
    }

    public Weapon GetWeaponOfType(Weapon.WeaponType type)
    {
        return weapons_[(int)type];
    }

    public void SwapToWeapon(int index)
    {
        if (index >= weapons_.Length || index < 0) return;

        weapons_[currentWeaponIndex_].gameObject.SetActive(false);
        currentWeaponIndex_ = index;
        weapons_[currentWeaponIndex_].gameObject.SetActive(true);
        UIManager.Instance.UpdateAmmo(weapons_[currentWeaponIndex_].TotalBulletAmmount);
        if(onWeaponSwap_ != null) onWeaponSwap_();
    }

    public void SwapToPreviousWeapon()
    {
        if (currentWeaponIndex_ - 1 < 0) return;

        weapons_[currentWeaponIndex_].gameObject.SetActive(false);
        currentWeaponIndex_--;
        weapons_[currentWeaponIndex_].gameObject.SetActive(true);
        UIManager.Instance.UpdateAmmo(weapons_[currentWeaponIndex_].TotalBulletAmmount);
        if(onWeaponSwap_ != null) onWeaponSwap_();
    }

    public bool IsSemiAutomaticWeapon()
    {
        return CurrentWeapon.Type != Weapon.WeaponType.kRifle;
    }
}