using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[CreateAssetMenu(menuName = "Actions/OpenURL")]
public class ActionOpenURL : ScriptableObject
{
    public void Load(string url)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        openWindow(url);
#else
        Application.OpenURL(url);
#endif
    }

    public void Load(StringVariable variable)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        openWindow(variable.Value);
#else
        Application.OpenURL(variable.Value);
#endif
    }

    [DllImport("__Internal")]
    private static extern void openWindow(string url);
}
