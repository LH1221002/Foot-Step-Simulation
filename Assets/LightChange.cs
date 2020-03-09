using HapticShoes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightChange : MonoBehaviour
{
    public Light light;
    public GameObject shoe;
    public GameObject LookAtTarget;
    public GameObject cont;
    public bool IsRightShoe = false;
    private ShoeController rightShoe;
    
    // Start is called before the first frame update
    public IEnumerator Start()
    {
        
     
        yield return new WaitForSeconds(2);
        Debug.Log("Los gehts");
        rightShoe = GetComponent<ShoeController>();
        light.color = Color.red;
        yield return new WaitForSeconds(1);
        rightShoe.CalibrateMin();

        yield return new WaitForSeconds(2);
        light.color = Color.green;
        yield return new WaitForSeconds(5);
        RaycastHit hit;
        
        var forward = transform.TransformDirection(Vector3.up) * 10;
        if (Physics.Raycast(transform.position, forward, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, forward, Color.yellow, 25);
            //Debug.Log("Did Hit");


            var forwarsd = -hit.transform.gameObject.transform.up * 10;
            var e = Quaternion.LookRotation(-hit.transform.up).eulerAngles;
            shoe.transform.rotation = Quaternion.FromToRotation(-Vector3.up, hit.normal);
            Debug.DrawRay(hit.transform.gameObject.transform.position, hit.normal * 10, Color.green, 25);

            //Debug.Log(LookAtTarget.transform.rotation.eulerAngles.y);


            cont.transform.LookAt(new Vector3(LookAtTarget.transform.position.x,cont.transform.position.y, LookAtTarget.transform.position.z));
            cont.transform.localRotation = Quaternion.Euler(0, cont.transform.localRotation.eulerAngles.y + 90, 0);
          
        }
        else
        {
            Debug.DrawRay(transform.position, forward, Color.white, 25);
            //Debug.Log("Did not Hit");
        }

        rightShoe.CalibrateMax();

        yield return new WaitForSeconds(3);
        light.color = Color.blue;
        

        rightShoe.ReceiveData(ChangeLight);
    }
    
    
    private void ChangeLight(int roh, int cal) {
        light.intensity = cal;
        //print(cal);
    }
}
