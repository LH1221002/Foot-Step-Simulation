using HapticShoes;
using System.Collections;
using UnityEngine;

/// <summary>
/// Example class on how to use the ShoeController
/// 
/// It shows a light on the shoe corresponding to the values of the pressure sensor
/// </summary>

public class LightChange : MonoBehaviour
{
    public Light lightIndicator;
    public GameObject shoe;
    public GameObject LookAtTarget;
    public GameObject cont;
    public Transform Controller;
    public Transform TrackerPoint;
    public bool isLeft;


    private ShoeController shoeController;

    private int currentPressure = 0;

    public IEnumerator Start()                             //needs to be an IEnumerator to be able to call WaitForSeconds (The ShoeController does need a few seconds to connect, before that, CalibrateMax() returns false)
    {
        yield return new WaitForSeconds(2);
        shoeController = GetComponent<ShoeController>();

        yield return new WaitForSeconds(2);
        lightIndicator.color = Color.green;                 //Indicator for the user to stand for calibration
        
        yield return new WaitForSeconds(2);

        RotateShoe();                                       //Rotates the 3D Shoe object in unity, so the controllers or trackers do not need to be at the exact same position every time (which would be practically impossible)

        while (shoeController!=null && !shoeController.CalibrateMax())
        {
            yield return new WaitForSeconds(1);
        }

        lightIndicator.color = Color.blue;                  //Indicator that the calibration is done

        if (shoeController) shoeController.ReceiveData(ChangeLight);            //Submits a method to be called with the pressure data in an update function

        lightIndicator.enabled = false;

        TrackerPoint.position = Controller.position;
    }

    private void RotateShoe()
    {
        var forward = Vector3.up * 50;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 10, transform.position.z), forward, out RaycastHit hit, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("ShoeCollider"))))
        {
            Debug.DrawRay(transform.position, forward, Color.yellow, 25);

            var forwarsd = -hit.transform.gameObject.transform.up * 10;
            var e = Quaternion.LookRotation(-hit.transform.up).eulerAngles;
            shoe.transform.rotation = Quaternion.FromToRotation(-Vector3.up, hit.normal);
            Debug.DrawRay(hit.transform.gameObject.transform.position, hit.normal * 10, Color.green, 25);

            cont.transform.LookAt(new Vector3(LookAtTarget.transform.position.x, cont.transform.position.y, LookAtTarget.transform.position.z));
            cont.transform.localRotation = Quaternion.Euler(0, cont.transform.localRotation.eulerAngles.y + (isLeft ? 75 : 105), 0);

        }
        else
        {
            Debug.DrawRay(transform.position, forward, Color.white, 25);
        }
    }


    private void ChangeLight(int roh, int cal)
    {
        lightIndicator.intensity = cal;
        currentPressure = roh;
        //print(cal);
    }
}
