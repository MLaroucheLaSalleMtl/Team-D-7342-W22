using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Items/Item Collection")]
public class ItemCollection : ScriptableObject
{
    [SerializeField]
    private InventoryItem[] items;

    public InventoryItem[] Items { get { return items; } }
}
