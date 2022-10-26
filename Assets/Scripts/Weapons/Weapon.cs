using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon")]
    public GameObject projectilePrefab_;
    public GameObject projectileSpawnRoot_;
    public float projectileSpeed_;
    public float totalAnimationTime_;
    [SerializeField]
    protected int totalBulletAmount_;
    public int TotalBulletAmmount { get { return totalBulletAmount_; } }

    [Header("Sound")]
    public AudioClip shotSound_;
    protected AudioSource audioSource_;

    public AnimationCurve animationCurve_;

    public delegate void OnShootDelegate();
    public event OnShootDelegate onShoot_;

    public delegate void OnShowAnimationDelegate();
    public event OnShowAnimationDelegate onShowAnimation_;
    public enum WeaponType
    {
        kPistol,
    }

    protected WeaponType type_;

    public WeaponType Type
    {
        get { return type_; }
    }

    protected bool isShowingAnimation_;
    protected float animationTime_;

    private void Awake()
    {
        Debug.Assert(projectilePrefab_ != null);
        Debug.Assert(projectileSpawnRoot_ != null);
        Debug.Assert(animationCurve_ != null);
    }

    private void Update()
    {
        
    }

    protected void OnShoot()
    {
        if(onShoot_ != null) onShoot_();
    }

    protected void OnShowAnimation()
    {
        if(onShowAnimation_ != null) onShowAnimation_();
    }

    public abstract void Shoot();
    public abstract void ShowAnimation();
    protected abstract void ReloadUpdate();
}