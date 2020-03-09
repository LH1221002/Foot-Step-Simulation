using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera currentCamera;
    void Update()
    {
        this.transform.LookAt(new Vector3(currentCamera.transform.position.x, transform.position.y, currentCamera.transform.position.z));
    }
}
