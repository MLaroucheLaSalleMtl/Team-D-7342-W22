using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class ScriptableEventListener<T> : MonoBehaviour
{
    protected abstract ScriptableEvent<T> ScriptableEvent { get; }

    protected abstract UnityEvent<T> Action { get; }

    public void Dispatch(T parameter)
    {
        Action?.Invoke(parameter);
    }

    public void OnEnable()
    {
        ScriptableEvent?.AddListener(this);
    }

    public void OnDisable()
    {
        ScriptableEvent?.RemoveListener(this);
    }

    /// <summary>
    /// Dispatch the last known parameter again
    /// </summary>
    public void ReDispatch()
    {
        if (ScriptableEvent.HasParameter)
        {
            Dispatch(ScriptableEvent.LastParameter);
        }
    }
}
