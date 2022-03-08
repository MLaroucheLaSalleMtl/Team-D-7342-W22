
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        item item = collision.GetComponent<item>();

        if (item != null)
        {
            //Get item details
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            //If item can be picked up
            if (itemDetails.canBePickedUp == true)
            {
                //Add item to inventory
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);
            }
        }
    }
}
