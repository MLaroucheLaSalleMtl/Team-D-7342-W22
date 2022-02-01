using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Item Action Dig Hole", menuName = "Items/Item Actions/Dig Hole")]
public class ItemAction_DigHole : ItemAction
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private ScriptableReference gridManagerReference;

    [SerializeField]
    private UnityEvent onSuccess;

    [SerializeField]
    private WeatherEvent weatherEvent;

    // Cache repeated use cases
    [System.NonSerialized]
    private GridManager gridManager;

    [System.NonSerialized]
    private EWeather currentWeather;
    
    public override IEnumerator ItemUseAction(Inventory userInventory, int itemIndex)
    {
        GridSelector gridSelector = userInventory.GetComponent<GridSelector>();

        if (gridManager == null)
        {
            gridManager = gridManagerReference.Reference?.GetComponent<GridManager>();
        }

        if (gridManager != null)
        {
            if (weatherEvent.HasParameter)
            {
                currentWeather = weatherEvent.LastParameter;
            }
        }

        if (gridSelector != null && gridManager != null)
        {
            Vector3Int selectionLocation = gridSelector.GetGridSelectionPosition();

            userInventory.GetComponent<Aimer>().LookAt(gridManager.Grid.CellToWorld(selectionLocation));

            Mover getMover = userInventory.GetComponent<Mover>();
            getMover?.FreezeMovement(true);

            BodyAnimation[] getEntityAnimator = userInventory.GetComponentsInChildren<BodyAnimation>();

            float animationTime = 0;

            for (int i = 0; i < getEntityAnimator.Length; i++)
            {
                animationTime = getEntityAnimator[i].ApplySmashAnimation(speed, userInventory.GetItem(itemIndex).Data.Icon);
            }

            gridSelector.SetFrozen(true);

            yield return new WaitForSeconds(animationTime * 0.5f);

            if (!gridManager.HasDirtHole(selectionLocation) && !gridManager.HasWater(selectionLocation) && gridManager.HasDirt(selectionLocation))
            {
                gridManager.SetDirtHoleTile(selectionLocation);

                if (currentWeather == EWeather.Rainy)
                {
                    gridManager.SetWateredDirtTile(selectionLocation);
                }

                onSuccess.Invoke();
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
