using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("Farming Kit/User Interface/Scroll Texture")]
[RequireComponent(typeof(RawImage))]
public class ScrollTexture : UIBehaviour
{
    [SerializeField]
    private RawImage target;

    [SerializeField]
    private Vector2 direction;

    private Vector2 baseSize;

    [SerializeField]
    private Vector2 offset;

    [SerializeField]
    private bool repeatX;

    [SerializeField]
    private bool repeatY;

    protected override void Awake()
    {
        target.material = new Material(target.material);
    }

    void Update ()
    {
        offset += direction * Time.deltaTime;

        target.material.SetTextureOffset("_MainTex", offset);
    }

    protected override void OnRectTransformDimensionsChange()
    {
        if (baseSize.x == 0 || baseSize.y == 0)
        {
            baseSize = target.rectTransform.sizeDelta;
        }


        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 sizeDifference = screenSize - baseSize;

        float xDifference = (repeatX)? sizeDifference.x / baseSize.x : 0;
        float yDifference = (repeatY)? sizeDifference.y / baseSize.y : 0;

        Rect newRect = target.uvRect;

        newRect.width = 1 + (xDifference);
        newRect.height = 1 + (yDifference);

        target.rectTransform.sizeDelta = baseSize + sizeDifference;
        target.uvRect = newRect;
    }
}
