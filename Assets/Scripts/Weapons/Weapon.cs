using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon")]
    public GameObject projectilePrefab_;
    public GameObject projectileSpawnRoot_;
    public float projectileSpeed_;
    public float reloadTime_;
    public int magazineMaxSize_;
    public int totalBulletAmount_;

    public AnimationCurve reloadAnimationCurve_;

    public delegate void OnShootDelegate();
    public event OnShootDelegate onShoot_;

    public delegate void OnReloadDelegate();
    public event OnReloadDelegate onReload_;

    protected int currentMagazineSize_;
    public int CurrentMagazineSize { get { return currentMagazineSize_; } }
    public enum WeaponType
    {
        kPistol,
    }

    protected WeaponType type_;

    public WeaponType Type
    {
        get { return type_; }
    }

    protected bool isReloading_;
    protected float reloadingTime_;

    private void Start()
    {
        Debug.Assert(projectilePrefab_ != null);
        Debug.Assert(projectileSpawnRoot_ != null);
        Debug.Assert(reloadAnimationCurve_ != null);
    }

    private void Update()
    {
        
    }

    protected void OnShoot()
    {
        if(onShoot_ != null) onShoot_();
    }

    protected void OnReload()
    {
        if(onReload_ != null) onReload_();
    }

    public abstract void Shoot();
    public abstract void Reload();
    protected abstract void ReloadUpdate();
}