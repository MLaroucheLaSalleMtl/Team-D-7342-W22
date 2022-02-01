using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

[AddComponentMenu("Farming Kit/Events/Warp Event Listener")]
public class WarpEventListener : MonoBehaviour
{
    [SerializeField]
    private WarpEvent scriptableEvent;

    [SerializeField]
    private UnityEventVector2 action;

    public void Dispatch(Vector2 parameter)
    {
        action?.Invoke(parameter);
    }

    public void OnEnable()
    {
        scriptableEvent?.AddListener(OnWarpEvent);
    }

    public void OnDisable()
    {
        scriptableEvent?.RemoveListener(OnWarpEvent);
    }

    private void OnWarpEvent(WarpLocation warpEvent)
    {
        Dispatch(warpEvent.Position);
    }
}
