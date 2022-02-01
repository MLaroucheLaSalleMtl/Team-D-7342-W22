using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IRecieveItemSlotIcon
{
    private int slotIndex;

    [System.Serializable]
    public class References
    {
        public TextMeshProUGUI SlotText;
        public TextMeshProUGUI AmountText;
        public Image Icon;
        public Image Highlight;
        public Inventory Inventory { get; set; }
        public ScriptableReference iconLayer;
        public Slider energySlider;
    }

    [System.Serializable]
    public class Settings
    {
        public bool displaySlotNumber = true;
        public bool equipItemOnClick = true;
        public bool moveItemOnDrag = false;
        public int slotIndexOffset = 1;
        public bool hasSelectionHighlight = false;
    }

    [SerializeField]
    private References references = new References();

    [SerializeField]
    private Settings settings = new Settings();

    private bool initialized = false;
    private bool isDragging = false;
    private bool hasEnergySlider = false;

    public int GetSlotIndex()
    {
        if (!initialized)
        {
            Initialize();
            return slotIndex;
        }
        else
        {
            return slotIndex;
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (!initialized)
        {
            slotIndex = transform.GetSiblingIndex() + settings.slotIndexOffset;

            if (settings.displaySlotNumber)
            {
                references.SlotText.gameObject.SetActive(true);
                references.SlotText.text = (slotIndex + 1).ToString();
            }
            else
            {
                references.SlotText.gameObject.SetActive(false);
            }

            references.Icon.gameObject.SetActive(false);
            references.AmountText.gameObject.SetActive(false);
            references.energySlider.gameObject.SetActive(false);

            SetHighlighted(false);

            initialized = true;
        }
    }

    public void OnRemoveItem(int index)
    {
        if (index == slotIndex)
        {
            references.Icon.gameObject.SetActive(false);
            references.AmountText.gameObject.SetActive(false);
            references.energySlider.gameObject.SetActive(false);
        }
    }

    public void OnInventoryInitialized(Inventory inventory)
    {
        references.Inventory = inventory;

        Initialize();
    }

    public void OnUseItem(int index, ItemData data, int amount)
    {
        if (index == slotIndex && data != null)
        {
            if (data.CanStack)
            {
                references.AmountText.gameObject.SetActive(true);

                references.AmountText.text = amount.ToString();
            }

            if (data.HasEnergy)
            {
                UpdateEnergySlider(index);
            }
        }
    }

    public void OnItemLoaded(int index, ItemData data, int amount)
    {
        if (index == slotIndex && data != null)
        {
            if (amount != 0)
            {
                references.AmountText.gameObject.SetActive(true);
                references.AmountText.text = amount.ToString();
            }
            else
            {
                references.AmountText.gameObject.SetActive(false);
            }

            if (data.Icon != null)
            {
                references.Icon.sprite = data.Icon;
                references.Icon.gameObject.SetActive(true);
            }
            else
            {
                references.Icon.gameObject.SetActive(false);
            }

            if (data.HasEnergy)
            {
                hasEnergySlider = true;
                references.energySlider.gameObject.SetActive(true);
                UpdateEnergySlider(index);
            }
            else
            {
                hasEnergySlider = false;
                references.energySlider.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateEnergySlider(int index)
    {
        if (references.energySlider != null)
        {
            references.energySlider.enabled = true;

            ItemEnergy currentEnergy = references.Inventory.GetItem(index).Energy;

            references.energySlider.minValue = currentEnergy.min;
            references.energySlider.maxValue = currentEnergy.max;

            references.energySlider.value = currentEnergy.current;
        }
    }

    private void SetHighlighted(bool set)
    {
        if (set)
        {
            references.Icon.transform.localScale = Vector3.one;
        }
        else
        {
            references.Icon.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        }

        references.Highlight.gameObject.SetActive(set);
    }

    public void OnItemSelect(int index, bool selected)
    {
        if (index == slotIndex)
        {
            if (settings.hasSelectionHighlight)
            {
                SetHighlighted(selected);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (settings.equipItemOnClick)
        {
            if (references.Inventory != null)
            {
                references.Inventory.SelectItemByIndex(slotIndex);
            }

            return;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (settings.moveItemOnDrag)
        {
            if (isDragging == false)
            {
                if (references.iconLayer != null)
                {
                    Transform iconLayerTransform = references.iconLayer.Reference.transform;

                    references.Icon.transform.SetParent(iconLayerTransform);
                    references.AmountText.transform.SetParent(iconLayerTransform);
                }

                isDragging = true;
            }

            Vector3 movePoint = eventData.position;

            references.Icon.transform.position = movePoint;
            references.AmountText.transform.position = movePoint;

            if (hasEnergySlider)
            {
                references.energySlider.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            OnDragStop();

            List<RaycastResult> raycastResult = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResult);

            for (int i = 0; i < raycastResult.Count; i++)
            {
                if (raycastResult[i].gameObject == this.gameObject)
                    return;

                IRecieveItemSlotIcon getInterface = raycastResult[i].gameObject.GetComponent<IRecieveItemSlotIcon>();

                if (getInterface != null)
                {
                    getInterface.OnRecieveItemIcon(slotIndex, references.Inventory);
                    return;
                }
            }

            references.Inventory.DropItem(slotIndex);
        }
    }

    private void OnDragStop()
    {
        references.AmountText.transform.SetParent(this.transform);
        references.AmountText.transform.localPosition = Vector2.zero;
        references.AmountText.transform.localScale = Vector3.one;

        references.Icon.transform.SetParent(this.transform);
        references.Icon.transform.localPosition = Vector2.zero;
        references.Icon.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

        if (hasEnergySlider)
        {
            references.energySlider.gameObject.SetActive(true);
        }

        isDragging = false;
    }

    public void OnDisable()
    {
        if (isDragging)
        {
            Invoke("OnDragStop", 0.01f);

            EventSystem getEventSystem = EventSystem.current;

            // Restart event system, could not find a more elegant solution.
            getEventSystem.enabled = false;
            getEventSystem.enabled = true;
        }
    }

    public void OnRecieveItemIcon(int index, Inventory sourceInventory)
    {
        // In case the source inventory does not match this inventory.
        // Tell the source to move it to the inventory linked to this one.
        if (sourceInventory != references.Inventory)
        {
            sourceInventory.MoveItem(index, slotIndex, references.Inventory);
        }
        else
        {
            sourceInventory.MoveItem(index, slotIndex);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Display item info
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide item info
    }
}
