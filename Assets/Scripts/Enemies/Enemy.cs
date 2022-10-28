using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Components")]
    public HealthComponent healthComponent_;


    protected virtual void Start()
    {
        healthComponent_ = GetComponent<HealthComponent>();
    }

    protected virtual void Update()
    {
        
    }
}
