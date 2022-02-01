using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InteractionField : MonoBehaviour
{

#if UNITY_EDITOR

    [MenuItem("GameObject/2D Object/Utility/Interaction Field")]
    static void CreateInteractionFieldObject()
    {
        Transform activeObjectTransform = Selection.activeGameObject?.transform;

        GameObject newObject = new GameObject("Interaction Field", typeof(InteractionField), typeof(BoxCollider2D));

        newObject.layer = LayerMask.NameToLayer("Interactable");

        newObject.GetComponent<BoxCollider2D>().isTrigger = true;

        if (activeObjectTransform != null)
        {
            newObject.transform.SetParent(activeObjectTransform.transform);
            newObject.transform.position = activeObjectTransform.transform.position;

            // Get the parent sprite and set the 2D collision according to that sprite.
            SpriteRenderer getParentSprite = activeObjectTransform.GetComponent<SpriteRenderer>();

            if (getParentSprite != null)
            {
                newObject.GetComponent<BoxCollider2D>().size = getParentSprite.size;
            }
        }

        Selection.activeGameObject = newObject;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(this.transform.position, useDistance);
    }

#endif

    [SerializeField]
    private bool useableFromAnyDistance = false;

    [SerializeField]
    private float useDistance = 0.1f;

    [SerializeField]
    private InteractionEvent onMouseInteractionEvent;

    [SerializeField]
    private UnityEvent interactAction;

    public bool IsWithinUseDistance(Vector2 worldPosition)
    {
        return useableFromAnyDistance || Vector2.Distance(worldPosition, this.transform.position) < useDistance;
    }

    public void Interact()
    {
        interactAction.Invoke();
    }

    private void OnMouseOver()
    {
        onMouseInteractionEvent?.Invoke(this);
    }

    private void OnMouseExit()
    {
        onMouseInteractionEvent?.Invoke(null);
    }


}
