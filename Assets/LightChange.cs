using HapticShoes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightChange : MonoBehaviour
{
    public Light light;

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
