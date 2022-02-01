using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Handles player interactions
/// </summary>
[AddComponentMenu("Farming Kit/Systems/Interaction System")]
public class InteractionSystem : GameSystem
{
    [SerializeField]
    private Texture2D cursorTexture;

    [SerializeField]
    private Texture2D cursorSelectableTexture;

    [SerializeField]
    private Texture2D cursorSelectableTransTexture;

    [SerializeField]
    private LayerMask selectionLayerMask;

    [SerializeField]
    private Vector2Event onPressInteractionButtonMouse;

    [SerializeField]
    private ScriptableReference playerReference;

    // Gets called by the interactable component
    [SerializeField]
    private InteractionEvent mouseInteraction;

    private InteractionField currentMouseInteraction;

    [SerializeField]
    private BoolEvent onPauzeGame;

    private RaycastHit2D[] raycastHits = new RaycastHit2D[5];

    private bool isPauzed;

    public override void OnLoadSystem()
    {
        if (cursorTexture != null)
        {
            UnityEngine.Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        }

#if UNITY_STANDALONE || UNITY_WEBGL
        mouseInteraction?.AddListener(OnMouseInteraction);
#endif

        onPressInteractionButtonMouse?.AddListener(OnMouseInteractionButton);
        onPauzeGame?.AddListener(OnPauzeGame);
    }

    private void OnPauzeGame(bool isPauzed)
    {
        this.isPauzed = isPauzed;

        if (isPauzed)
        {
            UnityEngine.Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
            currentMouseInteraction = null;
        }
    }

    private void OnMouseInteractionButton(Vector2 mousePosition)
    {
        OnPressInteractionButton();
    }

    private void OnPressInteractionButton()
    {
#if UNITY_STANDALONE
        // In case the mouse is already interacting with the object
        if (currentMouseInteraction != null)
        {
            currentMouseInteraction.Interact();

            return;
        }
#endif

        Vector2 playerAimPosition = playerReference.Reference.GetComponent<Aimer>().GetAimPosition();

        lastPos = playerAimPosition;

        TryInteract(playerAimPosition);
    }

    Vector2 lastPos = new Vector2(0, 0);

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(lastPos, 0.1f);
    }

    public void TryInteract(Vector2 worldPosition)
    {
        if (this.isPauzed)
            return;

        int foundInteractables = Physics2D.RaycastNonAlloc(worldPosition, Vector3.zero, raycastHits, 25, selectionLayerMask.value);

        if (foundInteractables > 0)
        {
            int cursorHitsLength = raycastHits.Length;

            for (int i = 0; i < cursorHitsLength; i++)
            {
                if (raycastHits[i].transform != null)
                {
                    InteractionField interactable = raycastHits[i].transform.GetComponent<InteractionField>();

                    if (interactable.IsWithinUseDistance(playerReference.Reference.transform.position))
                    {
                        interactable.Interact();
                    }
                }
            }
        }
    }

#if UNITY_STANDALONE || UNITY_WEBGL

    private void OnMouseInteraction(InteractionField interactable)
    {
        if (!this.isPauzed)
        {
            if (interactable == null)
            {
                UnityEngine.Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
                currentMouseInteraction = null;
            }
            else
            {
                if (interactable.IsWithinUseDistance(playerReference.Reference.transform.position))
                {
                    UnityEngine.Cursor.SetCursor(cursorSelectableTexture, Vector2.zero, CursorMode.ForceSoftware);
                    currentMouseInteraction = interactable;
                }
                else
                {
                    UnityEngine.Cursor.SetCursor(cursorSelectableTransTexture, Vector2.zero, CursorMode.ForceSoftware);
                    currentMouseInteraction = null;
                }
            }
        }
    }

#endif
}
