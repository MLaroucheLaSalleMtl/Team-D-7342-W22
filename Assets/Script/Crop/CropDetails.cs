using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription]
    public int seedItemCode;
    public int[] growthDays;
    public int totalGrowthDays;
    public GameObject[] growthPrefab;
    public Sprite[] growthSprite;
    public Season[] seasons;
    public Sprite harvestedSprite;

    [ItemCodeDescription]
    public int harvestedTransformItemCode;
    public bool hideCropBeforeHarvestedAnimation;
    public bool disableCropColliderBeforeHarvestedAnimation;
    public bool isHarvestedAnimation;
    public bool isHarvestActionEffect = false;
    public bool spawnCropProducedAtPlayerPosition;

    [ItemCodeDescription]
    public int[] harvestToolItemCode;
    public int[] requiredHarvestActions;

    [ItemCodeDescription]
    public int[] cropProducedItemCode;
    public int[] cropProducedMinQuantity;
    public int[] cropProducedMaxQuantity;
    public int daysToRegrow;

    //returns true if the tool item code can be used to harvest this crop, else returns false
    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //returns -1 if the tool can't be used to harvest this crop, else returns the number of harvest actions required by this tool
    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if (harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}
