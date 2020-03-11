using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DiamondScore : MonoBehaviour
{
    public TMP_Text[] text;
    public GameObject[] triangles;
    private int currentScore;
    public UnityEvent[] MethodsToCallAfterAllCollected;
    private int currentI;
    private int maxScore = 3;

    private void OnEnable()
    {
        UpdateTexts(Color.white);
        foreach(GameObject tri in triangles)
        {
            tri.SetActive(false);
        }
    }
    public void SetCurrentScore(int value)
    {
        currentScore = value;
        UpdateTexts(Color.white);
    }

    public void SetMaxScore(int value)
    {
        maxScore = value;
        UpdateTexts(Color.white);
        foreach (GameObject tri in triangles)
        {
            tri.SetActive(false);
        }
    }

    private void UpdateTexts(Color color)
    {
        foreach (TMP_Text txt in text)
        {
            txt.SetText($"{currentScore}/{maxScore}");
            txt.color = color;
        }
    }
    public bool IncrementScore()
    {
        currentScore++;
        print(currentScore);
        text?[0]?.SetText($"{currentScore}/{maxScore}");
        text?[1]?.SetText($"{currentScore}/{maxScore}");
        if (currentScore >= maxScore)
        {
            UpdateTexts(Color.cyan);
            foreach (GameObject tri in triangles)
            {
                tri.SetActive(true);
            }
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
