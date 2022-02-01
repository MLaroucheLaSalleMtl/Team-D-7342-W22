using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Display Item Storage Window")]
public class ActionDisplayItemStorageWindow : ScriptableObject
{
    [SerializeField]
    private ScriptableReference itemStorageWindow = null;

    [SerializeField]
    private ScriptableReference player = null;

    [SerializeField]
    private GameEvent requestPauze = null;

    public void Execute (Inventory targetInventory)
    {
        Transform itemStorageWindowTransform = itemStorageWindow.Reference?.transform;

        if (itemStorageWindowTransform != null)
        {
            for (int i = 0; i < itemStorageWindowTransform.childCount; i++)
            {
                if (itemStorageWindowTransform.GetChild(i).name == "Target")
                {
                    InventoryListener inventoryListener = itemStorageWindowTransform.GetChild(i).GetComponent<InventoryListener>();
                    inventoryListener.Target = targetInventory;
                }

                if (itemStorageWindowTransform.GetChild(i).name == "Source")
                {
                    InventoryListener inventoryListener = itemStorageWindowTransform.GetChild(i).GetComponent<InventoryListener>();
                    inventoryListener.Target = player?.Reference?.GetComponent<Inventory>();
                }
            }

            requestPauze?.Invoke();

            itemStorageWindowTransform.gameObject.SetActive(true);
        }
    }

}
