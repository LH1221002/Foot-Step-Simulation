using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor (typeof (ToggleMapAndSetup))]
public class ToggleMapAndSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();

        ToggleMapAndSetup myScript = (ToggleMapAndSetup)target;
        if(GUILayout.Button("Toggle"))
        {
            myScript.Toggle();
        }
    }
}
#endif
