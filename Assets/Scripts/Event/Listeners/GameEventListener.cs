using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[AddComponentMenu("Farming Kit/Events/Game Event Listener")]
public class GameEventListener : MonoBehaviour
{
    public GameEvent ScriptableEvent;

    public UnityEvent Action;

    public void Dispatch()
    {
        Action?.Invoke();
    }

    public void OnEnable()
    {
        ScriptableEvent?.AddListener(this);
    }

    public void OnDisable()
    {
        ScriptableEvent?.RemoveListener(this);
    }
}
