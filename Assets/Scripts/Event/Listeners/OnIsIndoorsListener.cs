using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class OnIsIndoorsListener : MonoBehaviour
{
    [SerializeField]
    private StringEvent warpEvent;

    [SerializeField]
    private UnityEvent isIndoors;

    [SerializeField]
    private UnityEvent isOutdoors;

    private void Awake()
    {
        warpEvent?.AddListener(OnSceneWarp);
    }

    private void OnSceneWarp(string sceneName)
    {
        if (IsIndoorScene(sceneName))
        {
            isIndoors.Invoke();
        }
        else
        {
            isOutdoors.Invoke();
        }
    }

    /// <summary>
    /// Checks if the path is within the Inside or Outside folder.
    /// </summary>
    private bool IsIndoorScene(string sceneName)
    {
        string getScenePath = SceneManager.GetSceneByName(sceneName).path;

        if (string.IsNullOrEmpty(getScenePath))
            return false;

        return getScenePath.Contains("InDoors");
    }
}
