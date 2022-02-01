using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[AddComponentMenu("Farming Kit/Events/Float Event Listener")]
public class FloatEventListener : ScriptableEventListener<float>
{
    [SerializeField]
    protected FloatEvent eventObject;

    [SerializeField]
    protected UnityEventFloat eventAction;

    protected override ScriptableEvent<float> ScriptableEvent
    {
        get
        {
            return eventObject;
        }
    }

    protected override UnityEvent<float> Action
    {
        get
        {
            return eventAction;
        }
    }
}
