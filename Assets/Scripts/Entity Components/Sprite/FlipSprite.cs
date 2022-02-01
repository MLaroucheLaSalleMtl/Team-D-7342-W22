using Lowscope.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Farming Kit/Entity Components/Sprites/Flip Sprite")]
[RequireComponent(typeof(SpriteRenderer))]
public class FlipSprite : MonoBehaviour, ISaveable
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private bool hasBeenFlipped = false;
    private bool isFlipped = false;

    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (!hasBeenFlipped)
        {
            bool flipState = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
            isFlipped = flipState;
            spriteRenderer.flipX = flipState;
        }
    }

    [System.Serializable]
    public struct SaveData
    {
        public bool hasBeenSaved;
        public bool isFlipped;
    }

    public void OnLoad(string data)
    {
        SaveData saveData = JsonUtility.FromJson<SaveData>(data);

        isFlipped = saveData.isFlipped;
        hasBeenFlipped = saveData.hasBeenSaved;

        spriteRenderer.flipX = isFlipped;
    }

    public string OnSave()
    {
        hasBeenFlipped = true;

        return JsonUtility.ToJson(new SaveData()
        {
            hasBeenSaved = (this.hasBeenFlipped = true),
            isFlipped = isFlipped
        });
    }

    public bool OnSaveCondition()
    {
        return !hasBeenFlipped;
    }
}
