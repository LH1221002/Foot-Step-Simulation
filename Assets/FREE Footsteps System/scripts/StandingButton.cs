using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StandingButton : MonoBehaviour
{
    public UnityEvent[] MethodsToCallAfterStanding;
    private int currentI;
    public float timeForButtonPress;
    private bool leftFootOnMe;
    private bool rightFootOnMe;

    public void Enter(bool left)
    {
        if (left)
        {
            leftFootOnMe = true;
        }
        else
        {
            rightFootOnMe = true;
        }
        if(leftFootOnMe && rightFootOnMe)
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
        int i = 0;
        while (leftFootOnMe && rightFootOnMe)
        {
            yield return new WaitForSeconds(timeForButtonPress / 8f);
            i++;
            if (i >= 8)
            {
                MethodsToCallAfterStanding[currentI].Invoke();
                currentI++;
                yield break;
            }
        }
    }
}
