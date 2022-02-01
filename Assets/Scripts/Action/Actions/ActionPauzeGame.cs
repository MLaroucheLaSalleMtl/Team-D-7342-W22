using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Actions/Pauze Game")]
public class ActionPauzeGame : ScriptableObject
{
    [SerializeField]
    private BoolEvent pauzeEvent;

    public void Execute(bool state)
    {
        pauzeEvent.Invoke(state);
    }

}
