using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [Header("Pickup Attributes")]
    public AnimationCurve bobbingMovementCurve_;
    public float bobbingSpeed_;
    public float rotationSpeed_;

    private float bobbingMovementInterval_;
    private float currentBobbingMovementTime_;
    private float bobbingOffset_;

    void Start()
    {
        currentBobbingMovementTime_ = 0.0f;
        bobbingOffset_ = transform.position.y;
        bobbingMovementInterval_ = bobbingMovementCurve_.keys[bobbingMovementCurve_.keys.Length - 1].time;
    }

    void Update()
    {
        PickupRotation();
        PickupMovement();
    }

    protected void PickupMovement()
    {

        currentBobbingMovementTime_ += bobbingSpeed_ * Time.deltaTime;
        transform.position = new Vector3 (transform.position.x, bobbingOffset_ + bobbingMovementCurve_.Evaluate(currentBobbingMovementTime_), transform.position.z);
        if(currentBobbingMovementTime_ >= bobbingMovementInterval_ || currentBobbingMovementTime_ <= 0.0f)
        {
            bobbingSpeed_ *= -1.0f;
        }
    }

    protected void PickupRotation()
    {
        transform.Rotate(new Vector3(0.0f, rotationSpeed_ *  Time.deltaTime, 0.0f));
    }
}
