using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using Lowscope.Saving.Components;
using Lowscope.Saving;

public class UISaveSlotDisplayer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerNameText;

    [SerializeField]
    private TextMeshProUGUI farmNameText;

    [SerializeField]
    private TextMeshProUGUI dateText;

    [SerializeField]
    private RawImage rawImage;

    [SerializeField]
    private GameObject characterRendererPrefab;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private GameObject slotAvailableObjects;

    [SerializeField]
    private GameObject slotUsedObjects;

    [SerializeField]
    private Button buttonRequestRemove;

    [SerializeField]
    private ScriptableReference confirmationWindow;

    [SerializeField]
    private UnityEvent OnRemoveRequested;

    [SerializeField]
    private UnityEventInt onPressedSlot = new UnityEventInt();

    [SerializeField]
    private DisplayCharacterBodyMetaData displayCharacterBodyData;

    [SerializeField]
    private StringVariable startingLevel;

    private int slot;

    private bool isInitialized;

    public void LoadSave()
    {
        if (slot == -1)
        {
            return;
        }

        string saveJson = "";
        SaveData saveData;
        if (SaveMaster.GetMetaData("savedata", out saveJson, slot))
        {
            saveData = JsonUtility.FromJson<SaveData>(saveJson);

            if (string.IsNullOrEmpty(saveData.lastScene))
            {
                saveData.lastScene = startingLevel.Value;
                SaveMaster.SetMetaData("savedata", JsonUtility.ToJson(saveData), slot);
            }

            if (!Application.CanStreamedLevelBeLoaded(saveData.lastScene))
            {
                confirmationWindow.Reference.GetComponent<ConfirmationWindow>().Configure(new ConfirmationWindow.Configuration()
                {
                    acceptOnly = true,
                    question = "Load Error \n (Scene file does not exist in build)",
                    answerYes = "Ok"
                });

                return;
            }
        }

        onPressedSlot.Invoke(slot);
    }

    private void Initialize()
    {
        isInitialized = true;

        if (characterRendererPrefab != null)
        {
            GameObject characterRenderer = GameObject.Instantiate(characterRendererPrefab);
            characterRenderer.transform.position = new Vector3((Camera.allCamerasCount) * 5, 0, 0);

            if (rawImage != null)
            {
                rawImage.texture = characterRenderer.GetComponent<UICharacterRenderer>().GetTexture();
                rawImage.SetNativeSize();
                rawImage.rectTransform.sizeDelta = rawImage.rectTransform.sizeDelta * 2;
            }

            displayCharacterBodyData = characterRenderer.GetComponentInChildren<DisplayCharacterBodyMetaData>();
        }

        buttonRequestRemove.onClick.AddListener(RequestRemoveSlot);
    }

    private void RequestRemoveSlot()
    {
        ConfirmationWindow getConfirmationWindow = confirmationWindow?.Reference?.GetComponent<ConfirmationWindow>();

        if (getConfirmationWindow != null)
        {
            getConfirmationWindow.Configure(new ConfirmationWindow.Configuration()
            {
                question = "Do you want to remove this save slot?",
                answerYes = "Yes",
                answerNo = "No",
                actionYes = OnActionRemove
            });
        }
    }

    private void OnActionRemove()
    {
        SaveMaster.DeleteSave(slot);
        LoadSlot(slot);
        OnRemoveRequested.Invoke();
    }

    public void LoadSlot(int slot)
    {
        this.slot = slot;

        if (!isInitialized)
        {
            Initialize();
        }

        if (!SaveMaster.IsSlotUsed(slot))
        {
            SetEmpty();
        }
        else
        {
            canvasGroup.alpha = 1f;

            string saveJson = "";
            SaveData saveData;

            if (SaveMaster.GetMetaData("savedata", out saveJson, slot))
            {
                saveData = JsonUtility.FromJson<SaveData>(saveJson);

                farmNameText?.SetText($"{saveData.farmName}");
                playerNameText?.SetText($"{saveData.playerName}");

                TimeSpan timeSpan = default;
                TimeSpan.TryParse(saveData.timePlayed, out timeSpan);
                dateText?.SetText($"{timeSpan.Hours.ToString("00")}:{timeSpan.Minutes.ToString("00")}");

                slotAvailableObjects.gameObject.SetActive(false);
                slotUsedObjects.gameObject.SetActive(true);

                displayCharacterBodyData.Load(slot);
            }
        }
    }

    public void SetEmpty()
    {
        this.slot = -1;

        canvasGroup.alpha = 0.5f;

        slotAvailableObjects.gameObject.SetActive(true);
        slotUsedObjects.gameObject.SetActive(false);
    }
}
