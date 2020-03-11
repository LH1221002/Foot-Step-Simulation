using HapticShoes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTrigger : MonoBehaviour
{
    ShoeController controller;
    void Awake()
    {
        controller = GetComponent<ShoeController>();
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log("TriggerEnter A");
        if (other != null && other.tag == "Material")
        {
            //Debug.Log("Getting Value ");
            var ob = other.gameObject;
            GroundProperties props = ob.GetComponent<GroundProperties>();
            if (controller != null)
            {
                Debug.Log("New Propeties " + props.volume + " " + props.strength);
                controller.SendToShoe(props.strength, props.material, props.volume, props.layer);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("TriggerExit A");
        if (other != null && other.tag == "Material")
        {
            if (controller != null)
                controller.SendToShoe(255, ShoeController.Material.Wood, 0);
        }
       
        
    }

}
