using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[AddComponentMenu("Farming Kit/Events/String Event Listener")]
public class StringEventListener : ScriptableEventListener<string>
{
    [SerializeField]
    protected StringEvent eventObject;

    [SerializeField]
    protected UnityEventString eventAction;

    protected override ScriptableEvent<string> ScriptableEvent
    {
        get
        {
            return eventObject;
        }
    }

    protected override UnityEvent<string> Action
    {
        get
        {
            return eventAction;
        }
    }
}
