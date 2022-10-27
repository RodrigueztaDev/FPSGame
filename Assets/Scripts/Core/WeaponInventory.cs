using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    public List<GameObject> weaponPrefabs_;
    private Weapon[] weapons_; // Using an array instead of a dictionary to have elements in order
    public Weapon CurrentWeapon
    {
        get { return weapons_[currentWeaponIndex_]; }
    }
    private int currentWeaponIndex_;

    void Start()
    {
        weapons_ = new Weapon[weaponPrefabs_.Count];
        Transform weaponRootTransform = GameObject.Find("PlayerWeaponRoot").transform;
        for(int i = 0; i < weapons_.Length; i++)
        {
            weapons_[i] = Instantiate(weaponPrefabs_[i], weaponRootTransform.position, Quaternion.identity).GetComponent<Weapon>();
            weapons_[i].transform.parent = weaponRootTransform;
            weapons_[i].gameObject.SetActive(false);
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
    }

    public void SwapToWeapon(int index)
    {
        if (index >= weapons_.Length || index < 0) return;

        weapons_[currentWeaponIndex_].gameObject.SetActive(false);
        currentWeaponIndex_ = index;
        weapons_[currentWeaponIndex_].gameObject.SetActive(true);
    }

    public void SwapToPreviousWeapon()
    {
        if (currentWeaponIndex_ - 1 < 0) return;

        weapons_[currentWeaponIndex_].gameObject.SetActive(false);
        currentWeaponIndex_--;
        weapons_[currentWeaponIndex_].gameObject.SetActive(true);
    }
}