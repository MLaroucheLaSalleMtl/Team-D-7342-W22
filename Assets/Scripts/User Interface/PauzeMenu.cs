using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Originally just a set of components.
// But a mix of events was required to make this work well.
public class PauzeMenu : MonoBehaviour
{
    [SerializeField]
    private GameEvent OnPressPauze;

    [SerializeField]
    private BoolEvent OnPauzeGame;

    [SerializeField, Tooltip("Target to influcence")]
    private GameObject target;

    private bool onPauzedGame;

    private void OnEnable()
    {
        OnPressPauze.AddListener(OnPauzePressed);
        OnPauzeGame.AddListener(OnGamePauzed);
    }

    private void OnDisable()
    {
        OnPressPauze.RemoveListener(OnPauzePressed);
        OnPauzeGame.RemoveListener(OnGamePauzed);
    }

    private void OnGamePauzed(bool state)
    {
        onPauzedGame = state;
    }

    private void OnPauzePressed()
    {
        Debug.Log("Pauze pressed");

        target.gameObject.SetActive(onPauzedGame);
    }
}
