using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(menuName = "Events/Warp Event")]
public class WarpEvent : ScriptableEvent<WarpLocation>
{
    internal void AddListener(WarpEvent onWarpRequest)
    {
        throw new NotImplementedException();
    }
}
