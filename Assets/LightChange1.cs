using HapticShoes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightChange1 : MonoBehaviour
{
    public Light lightIndicator;
    public GameObject shoe;
    public GameObject LookAtTarget;
    public GameObject cont;


    private ShoeController shoeController;

    private int currentPressure = 0;

    // Start is called before the first frame update
    public IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        //Debug.Log("Los gehts");
        shoeController = GetComponent<ShoeController>();

        yield return new WaitForSeconds(2);
        lightIndicator.color = Color.green;
        yield return new WaitForSeconds(3);
        RaycastHit hit;

        var forward = transform.TransformDirection(Vector3.up) * 10;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y+1, transform.position.z), forward, out hit, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("ShoeCollider"))))
        {
            Debug.DrawRay(transform.position, forward, Color.yellow, 25);
            //Debug.Log("Did Hit");


            var forwarsd = -hit.transform.gameObject.transform.up * 10;
            var e = Quaternion.LookRotation(-hit.transform.up).eulerAngles;
            shoe.transform.rotation = Quaternion.FromToRotation(-Vector3.up, hit.normal);
            Debug.DrawRay(hit.transform.gameObject.transform.position, hit.normal * 10, Color.green, 25);

            //Debug.Log(LookAtTarget.transform.rotation.eulerAngles.y);


            cont.transform.LookAt(new Vector3(LookAtTarget.transform.position.x, cont.transform.position.y, LookAtTarget.transform.position.z));
            cont.transform.localRotation = Quaternion.Euler(0, cont.transform.localRotation.eulerAngles.y + 90, 0);

        }
        else
        {
            Debug.DrawRay(transform.position, forward, Color.white, 25);
            //Debug.Log("Did not Hit");
        }

        shoeController.CalibrateMax();

        yield return new WaitForSeconds(3);
        lightIndicator.color = Color.blue;


        shoeController.ReceiveData(ChangeLight);
    }


    private void ChangeLight(int roh, int cal)
    {
        lightIndicator.intensity = cal;
        currentPressure = roh;
        //print(cal);
    }
}
