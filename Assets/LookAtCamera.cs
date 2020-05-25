using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera currentCamera;

    private void Start()
    {
        currentCamera = Camera.main;
    }
    void Update()
    {
        this.transform.LookAt(new Vector3(currentCamera.transform.position.x, transform.position.y, currentCamera.transform.position.z));
    }
}
