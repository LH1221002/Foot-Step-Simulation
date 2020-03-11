using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceMaterialChanger : MonoBehaviour
{
    private MeshRenderer mr;
    private Material current;
    public Material ToChange;

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        current = mr.material;
    }

    public void Enter()
    {
        mr.material = ToChange;
    }

    public void Exit()
    {
        mr.material = current;
    }
}
