using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerCamera : MonoBehaviour
{
    public float Scaler = 0.6f;
    void LateUpdate()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y * Scaler, transform.localPosition.z);
    }
}
