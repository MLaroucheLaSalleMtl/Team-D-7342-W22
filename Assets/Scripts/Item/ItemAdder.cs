using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;

[RequireComponent(typeof(Inventory))]
public class ItemAdder : MonoBehaviour, ISaveable
{
    [SerializeField]
    private List<InventoryItem> items = new List<InventoryItem>();

    private void Start()
    {
        if (!itemAdderSaveData.hasAddedItems)
        {
            Inventory getInventory = GetComponent<Inventory>();

            if (getInventory != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    getInventory.AddItem(items[i].Data, items[i].Amount);
                }
            }

            itemAdderSaveData.hasAddedItems = true;
        }
    }

    [System.Serializable]
    public struct ItemAdderSaveData
    {
        public bool hasAddedItems;
    }

    private ItemAdderSaveData itemAdderSaveData;

    public void OnLoad(string data)
    {
        itemAdderSaveData = JsonUtility.FromJson<ItemAdderSaveData>(data);
    }

    public string OnSave()
    {
        return JsonUtility.ToJson(itemAdderSaveData);
    }

    public bool OnSaveCondition()
    {
        return !itemAdderSaveData.hasAddedItems;
    }
}
