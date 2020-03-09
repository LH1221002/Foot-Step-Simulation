using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DiamondScore : MonoBehaviour
{
    public TMP_Text text;
    public int currentScore
    {
        get { return currentScore; }
        set
        {
            currentScore = value;
            text?.SetText($"{currentScore}/{maxScore}");
        }
    }
    public UnityEvent[] MethodsToCallAfterAllCollected;
    private int currentI;
    public int maxScore { get { return maxScore; } set
        {
            maxScore = value;
            text?.SetText($"{currentScore}/{maxScore}");
        } }

    public bool IncrementScore()
    {
        currentScore++;
        text?.SetText($"{currentScore}/{maxScore}");
        if (currentScore >= maxScore)
        {
            Next();
            return true;
        }
        else return false;
    }

    private void Next()
    {
        MethodsToCallAfterAllCollected[currentI].Invoke();
        currentI++;
    }
}
