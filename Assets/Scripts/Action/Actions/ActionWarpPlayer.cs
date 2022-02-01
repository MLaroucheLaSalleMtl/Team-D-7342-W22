using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Warp Player")]
public class ActionWarpPlayer : ScriptableAsset
{
    [SerializeField]
    private WarpEvent warpRequest;

    public void Execute(WarpLocation targetLocation)
    {
        warpRequest.Invoke(targetLocation);
    }
}
