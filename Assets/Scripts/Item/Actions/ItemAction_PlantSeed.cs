using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Item Action Plant Seed", menuName = "Items/Item Actions/Plant Seed")]
public class ItemAction_PlantSeed : ItemAction
{
    [SerializeField]
    private SaveablePrefab plantablePrefab;

    [SerializeField]
    private ScriptableReference gridManagerReference;

    private GridManager gridManager;

    public override IEnumerator ItemUseAction(Inventory userInventory, int itemIndex)
    {
        yield return null;

        InventoryItem getItem = userInventory.GetItem(itemIndex);

        if (gridManager == null)
        {
            gridManager = gridManagerReference.Reference.GetComponent<GridManager>();
        }

        if (PlantAction(userInventory))
        {
            getItem.Amount -= 1;

            if (getItem.Amount <= 0)
            {
                userInventory.RemoveItem(itemIndex);
            }
            else
            {
                userInventory.ReloadItemSlot(itemIndex);
            }
        }
    }

    private bool PlantAction(Inventory userInventory)
    {
        GridSelector gridSelector = userInventory.GetComponent<GridSelector>();
        Vector3Int selectionLocation = gridSelector.GetGridSelectionPosition();

        Vector3 loc = gridSelector.GetGridWorldSelectionPosition();

        RaycastHit2D[] findObjects = Physics2D.BoxCastAll(loc, gridManager.Grid.cellSize * 0.5f, 0, Vector2.zero, 50);

        // In case there is already a crop on this location, return.
        for (int i = 0; i < findObjects.Length; i++)
        {
            if (findObjects[i].transform.CompareTag("Crop"))
                return false;
        }

        if (gridManager.HasDirtHole(selectionLocation))
        {
            GameObject gameObject = plantablePrefab.Retrieve<GameObject>();
            gameObject.transform.position = gridManager.GetWorldLocation(selectionLocation);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool ItemUseCondition(Inventory userInventory, int itemIndex)
    {
        bool hasEnoughItems = userInventory.GetItem(itemIndex)?.Amount <= 0;

        if (!hasEnoughItems)
        {
            userInventory.RemoveItem(itemIndex);
        }

        return hasEnoughItems;
    }
}
