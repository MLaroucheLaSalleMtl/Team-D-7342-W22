using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// Makes it possible to click an canvas element.
/// Ensure that it has a component with raycast target enabled. (Image, text etc)
/// </summary>

[AddComponentMenu("Farming Kit/User Interface/Canvas Click Action")]
public class CanvasClickAction : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private UnityEvent onClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke();
    }
}
