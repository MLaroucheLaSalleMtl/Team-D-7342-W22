using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(SaveSystem))]
public class SaveSystemInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Go to persistent data folder"))
        {
            EditorUtility.OpenWithDefaultApp( Application.persistentDataPath);
        }
    }
}
