using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMapAndSetup : MonoBehaviour
{
    public enum Map{
        Map1,
        Map2
    }

    public enum VibrationType
    {
        DynamicVibration,
        StaticVibration
    }

    public enum InteractionType
    {
        Interactive,
        NonInteractive
    }

    public InteractionType Interaction;
    public VibrationType Vibration;
    public Map GameMap;

    public static bool UseStaticVibration;

    public GameObject[] AllObjects;

    public void Awake()
    {
        UseStaticVibration = Vibration == VibrationType.StaticVibration;
    }
    public void Toggle()
    {
        SettingActive<Map>(GameMap);
        //SettingActive<VibrationType>(Vibration);        //Not implemented yet
        SettingActive<InteractionType>(Interaction);
    }

    private void SettingActive<T>(T EnumValue) 
    {
        string[] EnumValues = System.Enum.GetNames(typeof(T));
        foreach(string str in EnumValues)
        {
            foreach (GameObject go in FindGameObjectsWithTag(str))
            {
                go.SetActive(str == EnumValue.ToString());
            }
        }
    }

    private GameObject[] FindGameObjectsWithTag(string str)
    {
        List<GameObject> ReturnValue = new List<GameObject>();
        foreach(GameObject go in AllObjects)
        {
            if (go == null) continue;
            if (go.tag == str) ReturnValue.Add(go);
        }
        return ReturnValue.ToArray();
    }
}
