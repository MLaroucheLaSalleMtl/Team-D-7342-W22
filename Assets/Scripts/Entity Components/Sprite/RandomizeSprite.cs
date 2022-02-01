using Lowscope.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Farming Kit/Entity Components/Sprites/Randomize Sprite")]
[RequireComponent(typeof(SpriteRenderer))]
public class RandomizeSprite : MonoBehaviour, ISaveable
{
    [SerializeField]
    private Sprite[] sprites = null;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private int currentIndex = -1;

    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (currentIndex == -1)
        {
            currentIndex = Random.Range(0, sprites.Length);
            SetSprite(currentIndex);

            isDirty = true;
        }
    }

    private void SetSprite (int index)
    {
        if (sprites == null)
        {
            Debug.Log("RandomizeSprite component has no sprites attached");
            return;
        }

        if (index < sprites.Length && index >= 0)
        {
            spriteRenderer.sprite = sprites[index];
        }
    }

    #region Saving

    private bool isDirty = false;

    public void OnLoad(string data)
    {
        int.TryParse(data, out currentIndex);
        SetSprite(currentIndex);
    }

    public string OnSave()
    {
        isDirty = false;
        return currentIndex.ToString();
    }

    public bool OnSaveCondition()
    {
        return isDirty;
    }

    #endregion
}
