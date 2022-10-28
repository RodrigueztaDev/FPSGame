using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Components")]
    public HealthComponent healthComponent_;

    [Header("Enemy Attributes")]
    public float materialChangeTime_ = 0.1f;

    private Material material_;
    private Color materialDefaultColor_;

    protected virtual void Start()
    {
        healthComponent_ = GetComponent<HealthComponent>();
        healthComponent_.onTakeDamage_ += () => OnTakeDamage();
        material_ = GetComponentInChildren<MeshRenderer>().material;
        materialDefaultColor_ = material_.color;
    }

    private void OnTakeDamage()
    {
        material_.color = Color.red;
        CancelInvoke();
        Invoke("DefaultMaterialColor", materialChangeTime_);
    }

    private void DefaultMaterialColor()
    {
        material_.color = materialDefaultColor_;
    }

    protected virtual void Update()
    {
        
    }
}
