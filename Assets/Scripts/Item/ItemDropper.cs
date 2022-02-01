using UnityEngine;
using System.Collections;

public class ItemDropper : MonoBehaviour
{
    [SerializeField]
    private ScriptablePoolContainer lootableItemPool;

    [System.Serializable]
    public class DroppableItem
    {
        [SerializeField, Tooltip("Data for the item to drop")]
        public ItemData itemData;

        [SerializeField, Tooltip("How much of a quantity should the lootable give?")]
        public Vector2Int itemAmount = new Vector2Int(1, 1);

        [SerializeField, Tooltip("How many lootables should be spawned?")]
        public Vector2Int dropCount = new Vector2Int(1, 1);
    }

    [SerializeField]
    private DroppableItem[] itemsToDrop;

    [SerializeField]
    private float dropRadius = 0.5f;

    public void Drop(Vector2 location)
    {
        for (int i = 0; i < itemsToDrop.Length; i++)
        {
            int dropCount = Random.Range(itemsToDrop[i].dropCount.x, itemsToDrop[i].dropCount.y);

            for (int i2 = 0; i2 < dropCount; i2++)
            {
                LootableItem lootableItem = lootableItemPool?.Retrieve<LootableItem>();

                if (lootableItem != null)
                {
                    int itemCount = Random.Range(itemsToDrop[i].itemAmount.x, itemsToDrop[i].itemAmount.y);

                    lootableItem.Configure(itemsToDrop[i].itemData, itemCount);
                    lootableItem.transform.position = location + ((Vector2)Random.insideUnitSphere * dropRadius);
                    lootableItem.gameObject.SetActive(true);
                }
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(this.transform.position, dropRadius);
    }
}
