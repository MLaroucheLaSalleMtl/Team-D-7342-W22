using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextApplicationVersionUpdater : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI text;

    private void Awake()
    {
        text?.SetText($"Version {Application.version}");
    }
}
