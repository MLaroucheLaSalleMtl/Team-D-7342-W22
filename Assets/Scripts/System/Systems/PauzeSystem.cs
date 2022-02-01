using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Handles pauze events
/// </summary>
[AddComponentMenu("Farming Kit/Systems/Pauze System")]
public class PauzeSystem : GameSystem
{
    [SerializeField]
    private GameEvent pauzeButton;

    [SerializeField]
    private BoolEvent pauzeEvent;

    [SerializeField]
    private BoolEvent displayPauzeMenu;

    [SerializeField]
    private GameEvent requestPauze;

    private bool isPauzed;

    public override void OnLoadSystem()
    {
        pauzeButton?.AddListener(OnPauzeButton);
        requestPauze?.AddListener(OnRequestPauze);
        pauzeEvent?.AddListener(OnPauze);
    }

    private void OnPauze(bool state)
    {
        isPauzed = state;
    }

    private void OnRequestPauze()
    {
        SetPauzed(true);
    }

    private void OnPauzeButton()
    {
        if (!isPauzed)
        {
            displayPauzeMenu.Invoke(true);
        }
        else
        {
            displayPauzeMenu.Invoke(false);
        }

        isPauzed = !isPauzed;
        SetPauzed(isPauzed);
    }

    public void SetPauzed(bool state)
    {
        isPauzed = state;
        pauzeEvent.Invoke(state);
    }
}
