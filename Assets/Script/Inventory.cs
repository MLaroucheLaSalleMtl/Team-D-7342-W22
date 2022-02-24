using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;
    // Start is called before the first frame update

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    #endregion

    public delegate void OnSlotCountChange(int val);
    public OnSlotCountChange onSlotCountChange;

    public delegate void OnChangeItem();
    public OnChangeItem onChangeItem;
    public List<Item> items = new List<Item>();
    private int slotCnt;

    public int SlotCnt
    {
        get =>slotCnt;
        set {
            slotCnt = value;
            onSlotCountChange.Invoke(slotCnt);
        }
    }

    void Start()
    {
        SlotCnt = 12;
    }

    // Update is called once per frame
    public bool AddItem(Item _item)
    {
        if(items.Count < SlotCnt)
        {
            items.Add(_item);
            if(onChangeItem != null)
            onChangeItem.Invoke();
            return true;

        }
        return false;

    }

private void OnTriggerEnter2D(Collider2D collision) 
{
      if(collision.gameObject.CompareTag("FieldItem"))
    {
        FieldItems fielditems = collision.GetComponent<FieldItems>();
        if(AddItem(fielditems.GetItem()))
        fielditems.DestroyItem();
    }
}
}
