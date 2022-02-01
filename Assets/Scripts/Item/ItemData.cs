using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Item Data", menuName = "Items/Item Data")]
public class ItemData : ScriptableAsset
{
    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private string itemName;

    [SerializeField]
    private string description;

    [SerializeField]
    private bool canStack;

    [SerializeField, Tooltip("Does this item go into an inventory slot?")]
    private bool hasSlot;

    [SerializeField, Tooltip("Does this item have a energy bar? Item actions are used to manage the values.")]
    private bool hasEnergy;

    [SerializeField, Tooltip("If has energy is toggled, these values will be utilized")]
    private ItemEnergy energyStartValue;

    [SerializeField, Tooltip("Can this item be dropped or destroyed?")]
    private bool isRemoveable;

    [SerializeField]
    private ItemAction action;

    public Sprite Icon
    {
        get { return icon; }
    }

    public string ItemName
    {
        get { return itemName; }
    }

    public ItemAction Action
    {
        get { return action; }
    }

    public bool CanStack
    {
        get { return canStack; }
    }

    public bool HasSlot
    {
        get { return hasSlot; }
    }

    public bool HasEnergy
    {
        get { return hasEnergy; }
    }

    public ItemEnergy EnergyStartValue
    {
        get { return energyStartValue; }
    }

    public bool IsRemoveable
    {
        get { return isRemoveable; }
    }
}
