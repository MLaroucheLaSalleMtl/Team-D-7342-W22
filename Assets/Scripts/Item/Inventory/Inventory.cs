using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lowscope.Saving;

public class Inventory : MonoBehaviour, ISaveable, IFreezeMovement
{
    [System.Serializable]
    public class InventoryEventDispatcher
    {
        private List<IInventoryLoaded> inventoryLoadInterfaces = new List<IInventoryLoaded>();
        private List<ILoadItem> loadInterfaces = new List<ILoadItem>();
        private List<ISelectItem> selectInterfaces = new List<ISelectItem>();
        private List<IMoveItem> moveInterfaces = new List<IMoveItem>();
        private List<IUseItem> useInterfaces = new List<IUseItem>();
        private List<IRemoveItem> removeInterfaces = new List<IRemoveItem>();
        private List<IDropItem> dropInterfaces = new List<IDropItem>();

        public InventoryEventDispatcher(GameObject gameObject, Inventory inventory)
        {
            GetAndAddInterfaces<IInventoryLoaded>(ref inventoryLoadInterfaces, gameObject);
            GetAndAddInterfaces<ILoadItem>(ref loadInterfaces, gameObject);
            GetAndAddInterfaces<ISelectItem>(ref selectInterfaces, gameObject);
            GetAndAddInterfaces<IMoveItem>(ref moveInterfaces, gameObject);
            GetAndAddInterfaces<IUseItem>(ref useInterfaces, gameObject);
            GetAndAddInterfaces<IRemoveItem>(ref removeInterfaces, gameObject);
            GetAndAddInterfaces<IDropItem>(ref dropInterfaces, gameObject);

            for (int i = 0; i < inventoryLoadInterfaces.Count; i++)
            {
                inventoryLoadInterfaces[i].OnInventoryLoaded(inventory);
            }

            for (int i = 0; i < loadInterfaces.Count; i++)
            {
                foreach (KeyValuePair<int, InventoryItem> item in inventory.items)
                {
                    loadInterfaces[i].OnItemLoaded(item.Key, item.Value.Data, item.Value.Amount);
                }
            }
        }

        private void GetAndAddInterfaces<T>(ref List<T> target, GameObject targetGameObject)
        {
            List<T> getInterfaces = new List<T>();
            targetGameObject.GetComponentsInChildren<T>(true, getInterfaces);

            if (getInterfaces.Count > 0)
            {
                target.AddRange(getInterfaces);
            }
        }

        public void DispatchInventoryInitialized(Inventory inventory)
        {
            int count = inventoryLoadInterfaces.Count;

            for (int i = 0; i < count; i++)
            {
                inventoryLoadInterfaces[i].OnInventoryLoaded(inventory);
            }
        }

        public void DispatchItemLoad(int slotIndex, ItemData data, int amount)
        {
            int count = loadInterfaces.Count;

            for (int i = 0; i < count; i++)
            {
                loadInterfaces[i].OnItemLoaded(slotIndex, data, amount);
            }
        }

        public void DispatchSelectItem(int index, bool selected)
        {
            int count = selectInterfaces.Count;

            for (int i = 0; i < count; i++)
            {
                selectInterfaces[i].OnItemSelect(index, selected);
            }
        }

        public void DispatchMoveItem(int fromIndex, int toIndex)
        {
            int count = moveInterfaces.Count;

            for (int i = 0; i < count; i++)
            {
                moveInterfaces[i].OnMoveItem(fromIndex, toIndex);
            }
        }

        public void DispatchUseItem(int index, ItemData data, int amount)
        {
            int count = useInterfaces.Count;

            for (int i = 0; i < count; i++)
            {
                useInterfaces[i].OnUseItem(index, data, amount);
            }
        }

        public void DispatchRemoveItem(int index)
        {
            int count = removeInterfaces.Count;

            for (int i = 0; i < count; i++)
            {
                removeInterfaces[i].OnRemoveItem(index);
            }
        }

        public void DispatchDropItem(int index, LootableItem lootable)
        {
            int count = dropInterfaces.Count;

            for (int i = 0; i < count; i++)
            {
                dropInterfaces[i].OnDropItem(index, lootable);
            }
        }

    }

    public Dictionary<int, InventoryEventDispatcher> eventDispatchers = new Dictionary<int, InventoryEventDispatcher>();

    [SerializeField]
    private int inventorySize = 30;

    public int InventorySize { get { return inventorySize; } }

    [SerializeField]
    private SaveablePrefab droppableItemPrefab = null;

    private List<InventoryItem> invisibleItems = new List<InventoryItem>();

    private Dictionary<int, InventoryItem> items = new Dictionary<int, InventoryItem>();

    private int selectedSlotIndex = -1;
    public int SelectedSlotIndex { get { return selectedSlotIndex; } }

    [SerializeField]
    private ItemCollection startingItems;

    private bool obtainedStartingItems;

    private void Start()
    {
        if (!obtainedStartingItems && startingItems != null)
        {
            foreach (InventoryItem item in startingItems.Items)
            {
                AddItem(item.Data, item.Amount);
            }

            obtainedStartingItems = true;
        }
    }

    #region Public Functionality

    public void AddListener(GameObject target)
    {
        int hashCode = target.GetHashCode();

        if (!eventDispatchers.ContainsKey(hashCode))
        {
            eventDispatchers.Add(hashCode, new InventoryEventDispatcher(target, this));
        }
    }

    public void RemoveListener(GameObject target)
    {
        int hashCode = target.GetHashCode();

        if (eventDispatchers.ContainsKey(hashCode))
        {
            eventDispatchers.Remove(hashCode);
        }
    }

    public void UseSelectedItem()
    {
        UseItem(selectedSlotIndex);
    }

    public void SelectItemByIndex(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > inventorySize || slotIndex == selectedSlotIndex)
            return;

        DeSelectItem(selectedSlotIndex);

        selectedSlotIndex = slotIndex;

        foreach (var dispatcher in eventDispatchers.Values)
        {
            dispatcher.DispatchSelectItem(slotIndex, true);
        }

        GetItem(slotIndex)?.Data?.Action?.ItemActiveAction(this, slotIndex);
    }

    public void SwitchItem(int direction)
    {
        SelectItemByIndex(selectedSlotIndex + direction);
    }

    public void DeSelectItem(int slotIndex)
    {
        GetItem(slotIndex)?.Data?.Action?.ItemUnactiveAction(this, slotIndex);

        foreach (var dispatcher in eventDispatchers.Values)
        {
            dispatcher.DispatchSelectItem(slotIndex, false);
        }
    }

    public InventoryItem GetItem(ItemData item, out int index)
    {
        if (item.HasSlot)
        {
            foreach (KeyValuePair<int, InventoryItem> getItem in items)
            {
                if (getItem.Value.Data == item)
                {
                    index = getItem.Key;
                    return getItem.Value;
                }
            }
        }
        else
        {
            for (int i = 0; i < invisibleItems.Count; i++)
            {
                if (invisibleItems[i].Data == item)
                {
                    index = i;
                    return invisibleItems[i];
                }
            }
        }

        index = -1;
        return null;
    }

    public void MoveItem(int slotIndexOne, int slotIndexTwo, Inventory targetInventory = null)
    {
        isDirty = true;

        InventoryItem itemOne = GetItem(slotIndexOne);
        InventoryItem itemTwo = (targetInventory == null) ? GetItem(slotIndexTwo) : targetInventory.GetItem(slotIndexTwo);

        bool itemOneValid = itemOne != null;
        bool itemTwoValid = itemTwo != null;

        // Item stacking
        if (itemOneValid && itemTwoValid)
        {
            if (itemTwo != null)
            {
                if (itemTwo.Data.CanStack)
                {
                    if (itemTwo.Data == itemOne.Data)
                    {
                        RemoveItem(slotIndexOne, true);

                        itemTwo.Amount += itemOne.Amount;
                        ReloadItemSlot(slotIndexTwo);
                    }
                }

                return;
            }
        }

        if (itemOneValid)
        {
            RemoveItem(slotIndexOne, true);
        }

        if (itemTwoValid)
        {
            if (targetInventory == null)
            {
                RemoveItem(slotIndexTwo, true);
            }
            else
            {
                targetInventory.RemoveItem(slotIndexTwo, true);
            }
        }

        if (itemOneValid)
        {
            if (targetInventory == null)
            {
                AddItem(itemOne.Data, itemOne.Amount, slotIndexTwo, false);
            }
            else
            {
                targetInventory.AddItem(itemOne.Data, itemOne.Amount, slotIndexTwo, false);
            }
        }

        if (itemTwoValid)
        {
            AddItem(itemTwo.Data, itemTwo.Amount, slotIndexOne, false);
        }
    }

    public InventoryItem GetItem(int index)
    {
        InventoryItem getItem;
        items.TryGetValue(index, out getItem);

        return getItem;
    }

    public void DropItem(int slotIndex)
    {
        if (!GetItem(slotIndex).Data.IsRemoveable)
            return;

        isDirty = true;

        LootableItem lootable = droppableItemPrefab.Retrieve<LootableItem>();

        if (lootable != null)
        {
            Aimer getAimer = this.GetComponent<Aimer>();
            Vector2 aimDirection = Vector2.zero;

            if (getAimer != null)
            {
                aimDirection = getAimer.GetAimDirection();
            }

            lootable.transform.position = (Vector2)this.transform.position + (aimDirection * (lootable.PickupDistance() * 1.05f));
            lootable.Configure(GetItem(slotIndex).Data, GetItem(slotIndex).Amount);
            lootable.gameObject.SetActive(true);

            foreach (var dispatcher in eventDispatchers.Values)
            {
                dispatcher.DispatchDropItem(slotIndex, lootable);
            }
        }

        RemoveItem(slotIndex);
    }

    /// <summary>
    /// Searches the inventory for a specific item and uses it.
    /// </summary>
    /// <param name="item"></param>
    public void UseItem(ItemData data)
    {
        foreach (KeyValuePair<int, InventoryItem> item in items)
        {
            if (item.Value.Data == data)
            {
                UseItem(item.Key);
                return;
            }
        }
    }

    public void UseItem(int slotIndex)
    {
        if (isMovementFrozen)
            return;

        IEnumerator itemActionIEnumerator = GetItem(slotIndex)?.Data?.Action?.ItemUseAction(this, slotIndex);

        if (itemActionIEnumerator != null)
        {
            StartCoroutine(itemActionIEnumerator);
        }

        InventoryItem getItem = GetItem(slotIndex);

        if (getItem != null)
        {
            foreach (var dispatcher in eventDispatchers.Values)
            {
                dispatcher.DispatchUseItem(slotIndex, getItem.Data, getItem.Amount);
            }
        }

        isDirty = true;
    }

    /// <summary>
    /// Reloads the item slot, this is useful if something has changed to the state of the item slot.
    /// Such as changes in energy.
    /// </summary>
    public void ReloadItemSlot(int slotIndex)
    {
        InventoryItem item = GetItem(slotIndex);

        bool foundItem = item != null;

        foreach (var dispatcher in eventDispatchers.Values)
        {
            if (foundItem)
            {
                // Tell all inventory listeners that this item, at this slot is loaded
                dispatcher.DispatchItemLoad(slotIndex, item.Data, item.Amount);
            }
            else
            {
                // Tell all inventory listeners that this item, at this slot is removed
                dispatcher.DispatchRemoveItem(slotIndex);
            }
        }
    }

    public void ReloadAllItemSlots()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            ReloadItemSlot(i);
        }
    }

    /// <summary>
    /// Add a item to the inventory
    /// </summary>
    /// <param name="data"> The item data assigned to the item </param>
    /// <param name="amount"> How much of this item do you want to add? In quantity to the slot. </param>
    /// <param name="slotIndex"> Do you want to assign it to a specific slot index? </param>
    /// <param name="scanForStack"> Will search through the inventory for similar item and adds it to the quantity  </param>
    /// <returns></returns>
    public bool AddItem(ItemData data, int amount, int slotIndex = -1, bool scanForStack = true)
    {
        isDirty = true;

        // Is item stackable? Then lets search the inventory first.
        if (scanForStack && data.CanStack)
        {
            int index;
            InventoryItem getItem = GetItem(data, out index);

            if (getItem != null)
            {
                getItem.Amount += amount;

                foreach (var dispatcher in eventDispatchers.Values)
                {
                    dispatcher.DispatchItemLoad(index, getItem.Data, getItem.Amount);
                }

                return true;
            }
        }

        if (data.HasSlot)
        {
            // Get an available slot if possible
            if (slotIndex == -1)
            {
                if (data.HasSlot)
                {
                    // Check if there are any free spaces left within the inventory
                    for (int i = 0; i < inventorySize; i++)
                    {
                        if (!items.ContainsKey(i))
                        {
                            slotIndex = i;

                            break;
                        }
                    }
                }
            }

            if (slotIndex != -1)
            {
                InventoryItem newItem = new InventoryItem()
                {
                    Amount = (data.CanStack) ? amount : 0,
                    Data = data,
                    Energy = data.EnergyStartValue
                };

                items.Add(slotIndex, newItem);

                newItem.Data?.Action?.ItemAcquisitionAction(this, slotIndex);

                foreach (var dispatcher in eventDispatchers.Values)
                {
                    dispatcher.DispatchItemLoad(slotIndex, data, (data.CanStack) ? amount : 0);
                }

                return true;
            }
        }
        else
        {
            InventoryItem newItem = new InventoryItem()
            {
                Amount = (data.CanStack) ? amount : 0,
                Data = data,
                Energy = data.EnergyStartValue
            };

            invisibleItems.Add(newItem);

            data.Action?.ItemAcquisitionAction(this, -1);

            foreach (var dispatcher in eventDispatchers.Values)
            {
                dispatcher.DispatchItemLoad(-1, data, (data.CanStack) ? amount : 0);
            }

            return true;
        }

        return false;
    }

    public void RemoveItem(int slotIndex, bool swapItem = false)
    {
        if (!GetItem(slotIndex).Data.IsRemoveable && !swapItem)
            return;

        isDirty = true;

        GetItem(slotIndex)?.Data?.Action?.ItemRemoveAction(this, slotIndex);

        items.Remove(slotIndex);

        foreach (var dispatcher in eventDispatchers.Values)
        {
            dispatcher.DispatchRemoveItem(slotIndex);
        }

    }

    #region Interface Implementations

    private bool isMovementFrozen;

    public void OnMovementFrozen(bool isMovementFrozen)
    {
        this.isMovementFrozen = isMovementFrozen;
    }

    #endregion

    #endregion

    #region Saving

    [System.Serializable]
    public struct InventoryItemSave
    {
        public int index;
        public string guidString;
        public int amount;
        public ItemEnergy energy;
    }

    [System.Serializable]
    public struct InventorySaveData
    {
        public bool obtainedStartingItems;
        public InventoryItemSave[] savedItems;
    }

    public InventorySaveData inventorySaveData;

    public string OnSave()
    {
        inventorySaveData = new InventorySaveData()
        {
            obtainedStartingItems = obtainedStartingItems,
            savedItems = new InventoryItemSave[items.Count + invisibleItems.Count]
        };

        int counter = 0;

        foreach (KeyValuePair<int, InventoryItem> item in items)
        {
            inventorySaveData.savedItems[counter] = new InventoryItemSave()
            {
                index = item.Key,
                guidString = item.Value.Data.GetGuid(),
                amount = item.Value.Amount,
                energy = item.Value.Energy
            };

            counter++;
        }

        foreach (InventoryItem item in invisibleItems)
        {
            inventorySaveData.savedItems[counter] = new InventoryItemSave()
            {
                index = 0,
                guidString = item.Data.GetGuid(),
                amount = item.Amount,
                energy = item.Energy
            };

            counter++;
        }

        return JsonUtility.ToJson(inventorySaveData);
    }

    public void OnLoad(string data)
    {
        inventorySaveData = JsonUtility.FromJson<InventorySaveData>(data);

        if (inventorySaveData.savedItems == null)
        {
            Debug.Log("saved items are null?");
            return;
        }

        if (inventorySaveData.savedItems.Length != 0)
        {
            items.Clear();

            for (int i = 0; i < inventorySaveData.savedItems.Length; i++)
            {
                InventoryItemSave getSave = inventorySaveData.savedItems[i];

                ItemData getItemData = ScriptableAssetDatabase.GetAsset(getSave.guidString) as ItemData;

                if (getItemData != null)
                {
                    AddItem(getItemData, getSave.amount, getSave.index);

                    // TODO: Create cleaner way to modify additional data in items.
                    if (getItemData.HasEnergy)
                    {
                        GetItem(getSave.index).Energy = getSave.energy;
                        ReloadItemSlot(getSave.index);
                    }
                }
                else
                {
                    Debug.Log($"Attempted to obtain guid: {getSave.guidString}");
                }
            }

            obtainedStartingItems = inventorySaveData.obtainedStartingItems;
        }
    }

    private bool isDirty;

    public bool OnSaveCondition()
    {
        return true;
    }

    #endregion
}
