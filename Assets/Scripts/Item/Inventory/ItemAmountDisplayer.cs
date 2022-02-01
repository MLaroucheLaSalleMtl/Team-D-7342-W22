using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemAmountDisplayer : MonoBehaviour, ILoadItem
{
    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private ItemData item;

    [SerializeField]
    private int addedLeadingZeroes = 0;

    public void OnItemLoaded(int index, ItemData data, int amount)
    {
        if (data == item)
        {
            text?.SetText(amount.ToString($"D{addedLeadingZeroes}"));
        }
    }
}
