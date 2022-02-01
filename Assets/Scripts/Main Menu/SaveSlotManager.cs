using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using Lowscope.Saving;
using Lowscope.Saving.Core;

public class SaveSlotManager : MonoBehaviour
{
    private UISaveSlotDisplayer[] saveSlotDisplayers;

    private List<int> saveGames;

    [SerializeField]
    private GameObject saveSlotContainer;

    [SerializeField]
    private Button buttonTabRight;

    [SerializeField]
    private Button buttonTabLeft;

    [SerializeField]
    private TextMeshProUGUI tabDisplayer;

    [SerializeField]
    private ScriptableReference confirmationWindow;

    private int currentTabIndex = 0;

    private void Awake()
    {
        saveSlotDisplayers = saveSlotContainer?.GetComponentsInChildren<UISaveSlotDisplayer>();

        buttonTabRight?.onClick.AddListener(OnSwitchTabRight);
        buttonTabLeft?.onClick.AddListener(OnSwitchTabLeft);
    }

    private void OnEnable()
    {
        RefreshSaveSlots();
    }

    public void RefreshSaveSlots()
    {
        saveGames = SaveFileUtility.GetUsedSlots().ToList();

        if (saveGames.Count == 0)
        {
            confirmationWindow.Reference?.GetComponent<ConfirmationWindow>().Configure(new ConfirmationWindow.Configuration()
            {
                acceptOnly = true,
                question = "No save games found",
                answerYes = "Okay"
            });

            this.gameObject.SetActive(false);

            return;
        }

        LoadSlots(currentTabIndex);
    }

    private void LoadSlots(int offset)
    {
        for (int i = 0; i < saveSlotDisplayers.Length; i++)
        {
            int slotIndex = i + (offset * saveSlotDisplayers.Length);

            if (slotIndex >= 0 && slotIndex < saveGames.Count)
            {
                saveSlotDisplayers[i].LoadSlot(saveGames[i]);
            }
            else
            {
                saveSlotDisplayers[i].SetEmpty();
            }

        }

        buttonTabLeft?.gameObject.SetActive(offset + 1 > 1);
        buttonTabRight?.gameObject.SetActive(saveGames.Count > saveSlotDisplayers.Length * (offset + 1));

        tabDisplayer?.transform.parent.gameObject.SetActive(saveGames.Count > saveSlotDisplayers.Length);
        tabDisplayer?.SetText($"Tab {offset + 1}");
    }

    private void OnSwitchTabLeft()
    {
        LoadSlots(currentTabIndex -= 1);
    }

    private void OnSwitchTabRight()
    {
        LoadSlots(currentTabIndex += 1);
    }
}
