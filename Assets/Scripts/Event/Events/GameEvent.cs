using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Events/Game Event")]
public class GameEvent : ScriptableObject, ISerializationCallbackReceiver
{
    private List<GameEventListener> eventListeners = new List<GameEventListener>();
    private List<System.Action> scriptEventListeners = new List<System.Action>();

    public void Invoke()
    {
        for (int i = 0; i < scriptEventListeners.Count; i++)
        {
            scriptEventListeners[i].Invoke();
        }

        for (int i = eventListeners.Count - 1; i >= 0; i--)
        {
            eventListeners[i].Dispatch();
        }
    }

    public void AddListener(System.Action listener)
    {
        scriptEventListeners.Add(listener);
    }

    public void RemoveListener(System.Action listener)
    {
        scriptEventListeners.Remove(listener);
    }

    public void AddListener(GameEventListener listener)
    {
        if (!eventListeners.Contains(listener))
        {
            eventListeners.Add(listener);
        }
    }

    public void RemoveListener(GameEventListener listener)
    {
        if (!eventListeners.Contains(listener))
        {
            eventListeners.Remove(listener);
        }
    }

    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
        eventListeners = new List<GameEventListener>();
        scriptEventListeners = new List<System.Action>();
    }
}
