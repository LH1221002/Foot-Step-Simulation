using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    public Vector3 velocity = Vector3.one;
    public float floorHeight = 0.0f;
    public float sleepThreshold = 0.05f;
    public float bounceCooef = 0.8f;
    public float gravity = -9.8f;

    void FixedUpdate()
    {
        if (velocity.magnitude > sleepThreshold || transform.position.y > floorHeight)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        transform.position += velocity * Time.fixedDeltaTime;
        if (transform.position.y <= floorHeight)
        {
            transform.position = new Vector3(transform.position.x, floorHeight, transform.position.z);
            velocity.y = -velocity.y;
            velocity *= bounceCooef;
        }
    }
}
