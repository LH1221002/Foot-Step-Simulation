using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DiamondScore : MonoBehaviour
{
    public TMP_Text text;
    private int currentScore;
    public UnityEvent[] MethodsToCallAfterAllCollected;
    private int currentI;
    private int maxScore = 3;

    public void SetCurrentScore(int value)
    {
        currentScore = value;
        text?.SetText($"{currentScore}/{maxScore}");
    }

    public void SetMaxScore(int value)
    {
        maxScore = value;
        text?.SetText($"{currentScore}/{maxScore}");
    }
    public bool IncrementScore()
    {
        currentScore++;
        print(currentScore);
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
