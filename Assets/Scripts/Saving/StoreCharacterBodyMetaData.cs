using Lowscope.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to store metadata for the character looks
/// This eliminates the use of loading a savegame beforehand, and instead
/// looks a small portion that contains some metadata about the save game.
/// </summary>
public class StoreCharacterBodyMetaData : MonoBehaviour
{
    [SerializeField] private BodySpriteSwapper[] bodySpriteSwappers;

    [System.Serializable]
    public class SaveData
    {
        public List<BodyData> bodyInfo = new List<BodyData>();
    }

    [System.Serializable]
    public class BodyData
    {
        public string saveId;
        public string data;
    }

    private void Reset()
    {
        bodySpriteSwappers = GetComponentsInChildren<BodySpriteSwapper>(true);
    }

    private void OnDestroy()
    {
        SaveData data = new SaveData();
        
        foreach (var item in bodySpriteSwappers)
        {
            data.bodyInfo.Add(new BodyData()
            {
                data = item.OnSave(),
                saveId = string.Format("{0}/{1}",item.transform.parent.name,item.name)
            });
        }

        SaveMaster.SetMetaData("character", JsonUtility.ToJson(data));
    }
}
