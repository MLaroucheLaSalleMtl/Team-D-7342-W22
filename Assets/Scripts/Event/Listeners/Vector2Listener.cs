using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[AddComponentMenu("Farming Kit/Events/Vector Event Listener")]
public class Vector2Listener : ScriptableEventListener<Vector2>
{
    public Vector2Event eventObject;

    public UnityEventVector2 eventAction = new UnityEventVector2();

    protected override ScriptableEvent<Vector2> ScriptableEvent
    {
        get
        {
            return eventObject;
        }
    }

    protected override UnityEvent<Vector2> Action
    {
        get
        {
            return eventAction;
        }
    }
}
