using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Equipment,
    Consumables,
    Etc

}

[System.Serializable]
public class Item
{
  public ItemType itemType;
  public string itemName;
 
  public Sprite itemImage;
    public List<ItemEffect> efts;
    public int itemCost;

  
  public bool Use()
  {
    bool isUsed = false;
    foreach(ItemEffect eft in efts)
    {
      isUsed = eft.ExecuteRole();
    }
      return isUsed;
  }
}
