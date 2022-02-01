using Lowscope.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCharacterBodyMetaData : MonoBehaviour
{
    [SerializeField] private BodySpriteSwapper[] bodySpriteSwappers;

    private void Reset()
    {
        bodySpriteSwappers = GetComponentsInChildren<BodySpriteSwapper>(true);
    }

    public void Load(int saveSlot)
    {
        string data;
        SaveMaster.GetMetaData("character", out data, saveSlot);

        if (!string.IsNullOrEmpty(data))
        {
            StoreCharacterBodyMetaData.SaveData d = JsonUtility.FromJson<StoreCharacterBodyMetaData.SaveData>(data);
            Dictionary<string, string> cachedData = new Dictionary<string, string>();
            foreach (var item in d.bodyInfo)
            {
                cachedData.Add(item.saveId, item.data);
            }

            foreach (var item in bodySpriteSwappers)
            {
                if (cachedData.TryGetValue(string.Format("{0}/{1}", item.transform.parent.name, item.name), out string bodyData))
                {
                    item.OnLoad(bodyData);
                }
            }
        }
    }
}
