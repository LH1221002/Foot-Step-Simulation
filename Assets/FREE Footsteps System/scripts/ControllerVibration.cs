using HapticShoes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerVibration : MonoBehaviour
{
    public SteamVR_Action_Vibration hapticAction;
    public SteamVR_Input_Sources source;
    public float duration = 0.25f;
    public float frequency = 100;
    public float amplitude = 75;

    
    void OnCollisionStay(Collision collisionInfo)
    {
        GetComponent<ShoeController>().SendVibrationToShoe(100);
        Pulse(duration, frequency, amplitude, source);
    }

    private void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source)
    {
        hapticAction.Execute(0, duration, frequency, amplitude, source);
        
    }

    public void HardPulse()
    {
        Pulse(1.5f, 300, 1, source);
    }
}
