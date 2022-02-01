using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Fade Screen")]
public class ActionFadeScreen : ScriptableObject
{
    [SerializeField]
    private ScriptableReference fadeScreenReference;

    public void FadeScreen(float duration)
    {
        if (fadeScreenReference != null)
        {
            fadeScreenReference.Reference.GetComponentInChildren<FadeImage>().Hide(duration);
            GameObject.DontDestroyOnLoad(fadeScreenReference.Reference);
        }
    }
}
