using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Button))]
public class DisableButtonOnEmptyStrings : MonoBehaviour
{
    private Button button;

    [SerializeField]
    private StringVariable[] strings;

    private void Awake()
    {
        button = this.GetComponent<Button>();
    }

    public void Verify ()
    {

        button.interactable = strings.All(s => !string.IsNullOrEmpty(s.Value));
    }
}
