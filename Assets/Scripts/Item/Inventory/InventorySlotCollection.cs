using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// In case you would like to have the calls on the slot directly without this component:
/// Add all the interfaces on this class to the item slot.
/// </summary>
public class InventorySlotCollection : MonoBehaviour, ILoadItem, IUseItem, IRemoveItem, ISelectItem, IInventoryLoaded
{
    private Dictionary<int, InventorySlot> slots = new Dictionary<int, InventorySlot>();
    private bool initialized = false;

    public void OnInventoryLoaded(Inventory inventory)
    {
        if (!initialized)
        {
            InventorySlot[] getSlots = GetComponentsInChildren<InventorySlot>(true);

            for (int i = 0; i < getSlots.Length; i++)
            {
                slots.Add(getSlots[i].GetSlotIndex(), getSlots[i]);
            }
            initialized = true;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].OnInventoryInitialized(inventory);
        }
    }

    public void OnItemLoaded(int index, ItemData data, int amount)
    {
        if (!data.HasSlot)
            return;

        GetSlot(index)?.OnItemLoaded(index, data, amount);
    }

    public void OnItemSelect(int index, bool selected)
    {
        GetSlot(index)?.OnItemSelect(index, selected);
    }

    public void OnRemoveItem(int index)
    {
        GetSlot(index)?.OnRemoveItem(index);
    }

    public void OnUseItem(int index, ItemData data, int amount)
    {
        GetSlot(index)?.OnUseItem(index, data, amount);
    }

    private InventorySlot GetSlot(int index)
    {
        InventorySlot slot;
        slots.TryGetValue(index, out slot);
        return slot;
    }
}
