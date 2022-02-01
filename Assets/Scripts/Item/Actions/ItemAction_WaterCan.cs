using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Item Action Water Can", menuName = "Items/Item Actions/Water Can")]
public class ItemAction_WaterCan : ItemAction
{
    [SerializeField]
    private float speed;

    // Manipulate the tile on this tilemap
    [SerializeField]
    private string targetTilemapName;

    // Only do the action the current tile coordinate has the tile map name
    [SerializeField]
    private string requiredTileMapName;

    // In case the current target is water, which we want to refill
    [SerializeField]
    private string waterTileMapName;

    [SerializeField]
    private ScriptableReference gridManagerReference;

    [SerializeField]
    private float energyCost;

    [SerializeField]
    private float waterEnergyRecovery;

    [SerializeField]
    private UnityEvent OnWateredGround;

    [SerializeField]
    private UnityEvent OnObtainedWater;

    // Cache repeated use cases

    [System.NonSerialized]
    private GridManager gridManager;

    public override IEnumerator ItemUseAction(Inventory userInventory, int itemIndex)
    {
        InventoryItem getInventoryItem = userInventory.GetItem(itemIndex);

        GridSelector gridSelector = userInventory.GetComponent<GridSelector>();

        if (gridManager == null)
        {
            gridManager = gridManagerReference.Reference?.GetComponent<GridManager>();
        }

        if (gridSelector != null && gridManager != null)
        {
            Vector3Int location = gridSelector.GetGridSelectionPosition();

            userInventory.GetComponent<Aimer>().LookAt(gridManager.Grid.CellToWorld(location));

            Mover getMover = userInventory.GetComponent<Mover>();
            getMover?.FreezeMovement(true);

            BodyAnimation[] getEntityAnimator = userInventory.GetComponentsInChildren<BodyAnimation>();

            float animationTime = 0;

            for (int i = 0; i < getEntityAnimator.Length; i++)
            {
                animationTime = getEntityAnimator[i].ApplyDropAnimation(speed, userInventory.GetItem(itemIndex).Data.Icon);
            }

            gridSelector.SetFrozen(true);

            yield return new WaitForSeconds(animationTime * 0.5f);

            if (!gridManager.HasWater(location))
            {
                if (!gridManager.HasWateredDirt(location) && gridManager.HasDirtHole(location))
                {
                    ItemEnergy currentItemEnergy = getInventoryItem.Energy;

                    if (currentItemEnergy.current >= energyCost)
                    {
                        gridManager.SetWateredDirtTile(location);

                        float newEnergy = currentItemEnergy.current - energyCost;
                        if (newEnergy < energyCost)
                        {
                            newEnergy = 0;
                        }

                        getInventoryItem.Energy = new ItemEnergy()
                        {
                            min = currentItemEnergy.min,
                            max = currentItemEnergy.max,
                            current = newEnergy
                        };

                        // Also do a sphere cast to ensure that any Day Count Listener is no longer frozen
                        // It becomes frozen when there is no watered ground, meaning the plant growth will stagnate.

                        Vector3 loc = gridSelector.GetGridWorldSelectionPosition();

                        RaycastHit2D[] findObjects = Physics2D.BoxCastAll(loc, gridManager.Grid.cellSize * 0.5f, 0, Vector2.zero, 50);

                        for (int i = 0; i < findObjects.Length; i++)
                        {
                            if (findObjects[i].transform.CompareTag("Crop"))
                            {
                                findObjects[i].transform?.GetComponent<DayCountDownListener>()?.FreezeCountDown(false);
                            }
                        }

                        OnWateredGround.Invoke();

                        userInventory.ReloadItemSlot(itemIndex);
                    }
                }
            }
            else
            {
                ItemEnergy currentItemEnergy = getInventoryItem.Energy;

                getInventoryItem.Energy = new ItemEnergy()
                {
                    min = currentItemEnergy.min,
                    max = currentItemEnergy.max,
                    current = Mathf.Clamp(currentItemEnergy.current + waterEnergyRecovery, currentItemEnergy.min, currentItemEnergy.max)
                };

                OnObtainedWater.Invoke();

                userInventory.ReloadItemSlot(itemIndex);
            }

            yield return new WaitForSeconds(animationTime * 0.5f);

            getMover.FreezeMovement(false);
            gridSelector.SetFrozen(false);

        }
    }

    public override bool ItemUseCondition(Inventory userInventory, int itemIndex)
    {
        return true;
    }

    public override void ItemActiveAction(Inventory userInventory, int itemIndex)
    {
        //userInventory.GetComponent<GridSelector>()?.Display(true);
    }

    public override void ItemUnactiveAction(Inventory userInventory, int itemIndex)
    {
        //userInventory.GetComponent<GridSelector>()?.Display(false);
    }
}
