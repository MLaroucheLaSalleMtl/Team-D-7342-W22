using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Farming Kit/Entity Components/Body/Sprite Setter")]
public class BodySpriteSetter : MonoBehaviour
{
    [SerializeField]
    private BodyData[] parts;

    [SerializeField]
    private BodySpriteSwapper spriteSwapper;

    [SerializeField]
    private UnityEventBodyData onBodyDataSet;

    public void Set (int index)
    {
        if (index >= 0 && index < parts.Length)
        {
            spriteSwapper.Set(parts[index]);

            onBodyDataSet.Invoke(parts[index]);
        }
    }
}
