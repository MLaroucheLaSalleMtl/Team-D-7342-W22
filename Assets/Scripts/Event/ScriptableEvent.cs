using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public abstract class ScriptableEvent<T> : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private bool dispatchLastStateOnAdd;

    [System.NonSerialized]
    private List<ScriptableEventListener<T>> eventListeners = new List<ScriptableEventListener<T>>();

    [System.NonSerialized]
    private List<System.Action<T>> scriptEventListeners = new List<System.Action<T>>();

    [System.NonSerialized]
    private T lastParameter;
    public T LastParameter
    {
        get { return lastParameter; }
    }

    [System.NonSerialized]
    private bool hasParameter;
    public bool HasParameter
    {
        get { return hasParameter; }
    }

    public void Invoke(T param)
    {
        for (int i = scriptEventListeners.Count - 1; i >= 0; i--)
        {
            scriptEventListeners[i].Invoke(param);
        }

        for (int i = eventListeners.Count - 1; i >= 0; i--)
        {
            eventListeners[i].Dispatch(param);
        }

        if (dispatchLastStateOnAdd)
        {
            lastParameter = param;
            hasParameter = true;
        }
    }

    public void AddListener(System.Action<T> listener)
    {
        scriptEventListeners.Add(listener);

        if (dispatchLastStateOnAdd && hasParameter)
        {
            listener.Invoke(lastParameter);
        }
    }

    public void RemoveListener(System.Action<T> listener)
    {
        scriptEventListeners.Remove(listener);
    }

    public void AddListener(ScriptableEventListener<T> listener)
    {
        if (!eventListeners.Contains(listener))
        {
            eventListeners.Add(listener);

            if (dispatchLastStateOnAdd && hasParameter)
            {
                listener.Dispatch(lastParameter);
            }
        }
    }

    public void RemoveListener(ScriptableEventListener<T> listener)
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

    }
}