using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float time_;

    void Start()
    {
        Destroy(gameObject, time_);
    }
}
