using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{

    Inventory inven;

    public GameObject inventoryPanel;
    bool activeInventory = false;
    
    public Slot[] slots;
    public Transform slotHolder;
    // Start is called before the first frame update
    private void Start()
    {
        inven = Inventory.instance;
        slots = slotHolder.GetComponentsInChildren<Slot>();
        inven.onSlotCountChange += SlotChange;
        inven.onChangeItem += RedrawSlotUI;
        RedrawSlotUI();
        inventoryPanel.SetActive(activeInventory);
        closeShop.onClick.AddListener(DeActiveShop);
    }
    
    private void SlotChange(int val)
    {
       for(int i = 0; i < slots.Length; i++ )
       {
           slots[i].slotnum = i;
           if(i<inven.SlotCnt)
           slots[i].GetComponent<Button>().interactable = true;
           else
           slots[i].GetComponent<Button>().interactable = false;
       }
    }



    // Update is called once per frame
    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.I) && !isStoreActive){
            activeInventory = !activeInventory;
            inventoryPanel.SetActive(activeInventory);
        }
        if(Input.GetMouseButtonUp(0))
        {
            RayShop();
        }
    }

    public void AddSlot()
    {
        inven.SlotCnt += 12;
    }

    private void RedrawSlotUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveSlot();
        }
        for(int i = 0; i < inven.items.Count; i++)
        {
            slots[i].item = inven.items[i];
            slots[i].UpdateSlotUI();
        }
    }

    public GameObject shop;
    public Button closeShop;
    public bool isStoreActive;
    public void RayShop()
    {
        Vector3 mousePos= Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -10;
        Debug.DrawRay(mousePos, transform.forward, Color.red, 0.5f);
        if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1))
        {
            RaycastHit2D hit2D = Physics2D.Raycast(mousePos,transform.forward,30);
        if(hit2D.collider !=null)
        {
            if(hit2D.collider.CompareTag("Store"))
            {
                if(!isStoreActive)
                {
                ActiveShop(true);
                }
            }
        }
        }
    }
    public void ActiveShop(bool isOpen)
    {
        if(!activeInventory)
        {
        isStoreActive = isOpen;
        shop.SetActive(isOpen);
        inventoryPanel.SetActive(isOpen);
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].isShopMode = isOpen;
            }
        }
    }

    public void DeActiveShop()
    {
        ActiveShop(false);
    }

    public void SellBtn()
    {
        for (int i = slots.Length; i> 0; i--)
        {
            slots[i-1].SellItem();
        }
    }
    
}
