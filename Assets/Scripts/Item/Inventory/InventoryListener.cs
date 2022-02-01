using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryListener : MonoBehaviour
{
    [SerializeField]
    private Inventory target;

    public Inventory Target
    {
        get { return target; }
        set
        {
            if (target != null && target != value)
            {
                UnSubscribeToAllInventories();
            }

            target = value;
            target?.AddListener(this.gameObject);
            target.ReloadAllItemSlots();
        }
    }

    private List<Inventory> obtainedInventories = new List<Inventory>();

    private void OnEnable()
    {
        if (target != null)
        {
            target?.AddListener(this.gameObject);
        }
    }

    private void OnDisable()
    {
        UnSubscribeToAllInventories();
    }

    private void UnSubscribeToAllInventories()
    {
        for (int i = 0; i < obtainedInventories.Count; i++)
        {
            obtainedInventories[i].RemoveListener(this.gameObject);
            obtainedInventories.Remove(obtainedInventories[i]);
        }

        if (target != null && this.gameObject != null)
            target.RemoveListener(this.gameObject);
    }
}
