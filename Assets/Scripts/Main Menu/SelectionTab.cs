using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectionTab : MonoBehaviour
{
    [SerializeField]
    private Button buttonLeft = null;

    [SerializeField]
    private Button buttonRight = null;

    [SerializeField]
    private TextMeshProUGUI title = null;

    [System.Serializable]
    public class Selection
    {
        public string tabName;
        public UnityEvent OnSelection;
    }

    [SerializeField]
    private Selection[] selections;

    [SerializeField]
    private UnityEventInt OnSelectionChange = new UnityEventInt();

    private int currentIndex = 0;

    private void Awake()
    {
        buttonLeft.onClick.AddListener(() => { OnSwitchSelection(-1); });
        buttonRight.onClick.AddListener(() => { OnSwitchSelection(1); });
    }

    private void Start()
    {
        OnSwitchSelection(0);
    }

    private void OnSwitchSelection(int direction)
    {
        currentIndex += direction;

        if (currentIndex > selections.Length - 1)
        {
            currentIndex = 0;
        }

        if (currentIndex < 0)
        {
            currentIndex = selections.Length - 1;
        }

        title.SetText(selections[currentIndex].tabName);
        selections[currentIndex].OnSelection.Invoke();

        OnSelectionChange.Invoke(currentIndex);
    }
}
