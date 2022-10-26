using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public LayerMask ignoreLayers = Physics.DefaultRaycastLayers;

    private Vector3 lastPosition_;

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        Vector3 position = transform.position;
        RaycastHit hit;

        float distance = Vector3.Distance(lastPosition_, position);

        if (Physics.Raycast(position, (lastPosition_ - position).normalized, out hit, distance, ignoreLayers))
        {
            Kill();
        }
        lastPosition_ = position;
    }

    public void Shoot(Vector3 direction, float speed)
    {
        lastPosition_ = transform.position;
        GetComponent<Rigidbody>().AddForce(direction * speed, ForceMode.VelocityChange);
        Invoke("Kill", 5.0f);
    }

    private void Kill()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.layer != 10) Kill();
    }
}
