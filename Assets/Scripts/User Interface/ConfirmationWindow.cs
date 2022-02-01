using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationWindow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textQuestion;

    [SerializeField]
    private TextMeshProUGUI textAnswerYes;

    [SerializeField]
    private TextMeshProUGUI textAnswerNo;

    [SerializeField]
    private TextMeshProUGUI textAccept;

    [SerializeField]
    private Button buttonYes;

    [SerializeField]
    private Button buttonNo;

    [SerializeField]
    private Button buttonAccept;

    private System.Action actionYes;
    private System.Action actionNo;

    public struct Configuration
    {
        public bool acceptOnly;
        public string question;
        public string answerYes;
        public string answerNo;
        public System.Action actionYes;
        public System.Action actionNo;
    }

    private void Awake()
    {
        buttonYes?.onClick.AddListener(OnClickYesButton);
        buttonAccept?.onClick.AddListener(OnClickYesButton);
        buttonNo?.onClick.AddListener(OnClickNoButton);
    }

    private void OnClickNoButton()
    {
        if (actionNo != null)
        {
            actionNo.Invoke();
        }

        this.gameObject.SetActive(false);
        ClearEvents();
    }

    private void OnClickYesButton()
    {
        if (actionYes != null)
        {
            actionYes.Invoke();
        }

        this.gameObject.SetActive(false);
        ClearEvents();
    }

    public void Configure(Configuration configuration)
    {
        textQuestion?.SetText(configuration.question);
        textAnswerYes?.SetText(configuration.answerYes);
        textAnswerNo?.SetText(configuration.answerNo);
        textAccept?.SetText(configuration.answerYes);

        buttonYes?.gameObject.SetActive(!configuration.acceptOnly);
        buttonNo?.gameObject.SetActive(!configuration.acceptOnly);
        buttonAccept?.gameObject.SetActive(configuration.acceptOnly);

        if (configuration.actionYes != null)
        {
            this.actionYes += configuration.actionYes;
        }

        if (configuration.actionNo != null)
        {
            this.actionNo += configuration.actionNo;
        }

        this.gameObject.SetActive(true);
    }

    private void ClearEvents()
    {
        if (actionYes != null)
        {
            foreach (Delegate d in actionYes.GetInvocationList())
            {
                actionYes -= (System.Action)d;
            }
        }

        if (actionNo != null)
        {
            foreach (Delegate d in actionNo.GetInvocationList())
            {
                actionNo -= (System.Action)d;
            }
        }
    }
}
