using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StandingButton : MonoBehaviour
{
    public UnityEvent[] MethodsToCallAfterStanding;
    public PlatformTransitioner platformTransitioner;
    private int currentI;
    public float timeForButtonPress;
    private bool leftFootOnMe;
    private bool rightFootOnMe;
    private bool running;

    public void Enter(bool left)
    {
        print("Button Enter");
        if (leftFootOnMe && rightFootOnMe) return;
        if (left)
        {
            leftFootOnMe = true;
        }
        else
        {
            rightFootOnMe = true;
        }
        if(leftFootOnMe && rightFootOnMe && !running)
        {
            StartCoroutine(ButtonPress());
        }
    }

    public void Exit(bool left)
    {
        if (left)
        {
            leftFootOnMe = false;
        }
        else
        {
            rightFootOnMe = false;
        }
    }
    private IEnumerator ButtonPress()
    {
        running = true;
        Debug.LogWarning("Starting");
        int i = 0;
        while (leftFootOnMe && rightFootOnMe)
        {
            yield return new WaitForSeconds(timeForButtonPress / 8f);
            i++;
            if (i >= 8)
            {
                Debug.LogWarning("Ending");
                //MethodsToCallAfterStanding[currentI].Invoke();
                platformTransitioner.afterUp = MethodsToCallAfterStanding[currentI];
                platformTransitioner.DoDownAnimation();
                currentI++;
                running = false;
                yield break;
            }
        }
        running = false;
        yield break;
    }
}
