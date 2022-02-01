using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;

[AddComponentMenu("Farming Kit/Entity Components/Body/Sprite Swapper")]
[RequireComponent(typeof(SpriteRenderer))]
public class BodySpriteSwapper : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Checks if the target has sprite names that are the same with the data.
    /// If this is true, then it will replace them.
    /// </summary>

    [SerializeField]
    private bool hideWhenNoData;

    [SerializeField]
    private BodyData data;

    public BodyData Data
    {
        set
        {
            data = value;
            ClearHistory();
        }

        get
        {
            return data;
        }
    }

    private SpriteRenderer target;

    private string currentFrame;

    private Sprite lastSprite;

    private Dictionary<Sprite, string> cachedSpriteNames = new Dictionary<Sprite, string>();

    public void ClearHistory()
    {
        currentFrame = string.Empty;
        lastSprite = null;
    }

    // It is required to cache the sprite names, this is because getting a name causes a GC allocation
    private string GetSpriteName(Sprite sprite)
    {
        string name;
        cachedSpriteNames.TryGetValue(sprite, out name);

        if (string.IsNullOrEmpty(name))
        {
            name = sprite.name;
            cachedSpriteNames.Add(target.sprite, name);
            return name;
        }
        else
        {
            return name;
        }
    }

    public void Execute()
    {
        if (target == null)
            return;

        // If the current frame has not changed, use the previous one.
        if (target.sprite != null && string.Equals(currentFrame, GetSpriteName(target.sprite), System.StringComparison.Ordinal))
        {
            target.sprite = lastSprite;
            return;
        }

        if (data == null || target.sprite == null)
            return;

        currentFrame = GetSpriteName(target.sprite);

        int substringIndex = currentFrame.LastIndexOf('_') + 1;

        string getDirection = currentFrame.Substring(substringIndex, (currentFrame.Length - substringIndex) - 1);

        int index = -1;

        int.TryParse(currentFrame[currentFrame.Length - 1].ToString(), out index);

        if (index != -1)
        {
            switch (getDirection)
            {
                case "side":

                    if (index < data.side.Count)
                    {
                        target.sprite = data.side[index];
                    }

                    break;
                case "front":

                    if (index < data.front.Count)
                    {
                        target.sprite = data.front[index];
                    }

                    break;
                case "back":

                    if (index < data.back.Count)
                    {
                        target.sprite = data.back[index];
                    }

                    break;

                default:
                    break;
            }
        }

        lastSprite = target.sprite;

    }

    public void Set(BodyData data)
    {
        Data = data;
        HideOnEmptyData();

        isDirty = true;
    }

    public void Set(BodyDataVariable data)
    {
        Data = data.Value;
        HideOnEmptyData();

        isDirty = true;
    }

    private void Awake()
    {
        target = this.GetComponent<SpriteRenderer>();
        HideOnEmptyData();
    }

    private void HideOnEmptyData()
    {
        if (target == null)
        {
            target = this.GetComponent<SpriteRenderer>();
        }

        if (hideWhenNoData)
        {
            target.enabled = data != null;
            this.enabled = data != null;
        }
    }

    public void LateUpdate()
    {
        Execute();
    }

    public void OnValidate()
    {
        ClearHistory();

        LateUpdate();
    }

    [System.Serializable]
    public struct SaveData
    {
        public string partGuid;
    }

    public string OnSave()
    {
        return JsonUtility.ToJson(new SaveData()
        {
            partGuid = Data?.GetGuid()
        });
    }

    public void OnLoad(string data)
    {
        SaveData saveData = JsonUtility.FromJson<SaveData>(data);

        if (!string.IsNullOrEmpty(saveData.partGuid))
        {
            Set(ScriptableAssetDatabase.GetAsset(saveData.partGuid) as BodyData);
        }
    }

    private bool isDirty;

    public bool OnSaveCondition()
    {
        if (isDirty)
        {
            isDirty = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}
