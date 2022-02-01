using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[AddComponentMenu("Farming Kit/Entity Components/Player/Grid Selector")]
public class GridSelector : MonoBehaviour, IMove
{
    [SerializeField]
    private ScriptableReference gridManagerReference;

    private GridManager gridManager;

    [SerializeField]
    private BoolEvent onGamePauzed;

    [SerializeField]
    private Vector2Event mouseWorldInput;

    [SerializeField]
    private Sprite cursorSprite;

    private GameObject selectionGameObject;
    private SpriteRenderer selectionSpriteRenderer;

    [SerializeField]
    private float selectionTileDistance;

    [SerializeField]
    private Vector2 gridOffset = new Vector3(-0.08f, -0.08f);

    [SerializeField]
    private Vector2 selectionViewOffset;

    private Vector3Int characterForwardGridLocation;
    private Vector3Int characterGridLocation;
    private Vector3Int mouseGridLocation;

    private Vector3Int currentSelectionGridPosition;

    private Vector2 lastMousePosition;

    private bool displaySelectionView;
    private bool frozen;
    private Vector2 lastMoveDirection;

    private void Awake()
    {
        mouseWorldInput?.AddListener(OnMouseMove);
        onGamePauzed?.AddListener(OnGamePauze);

        selectionGameObject = new GameObject();
        selectionGameObject.transform.SetParent(this.transform);
        selectionSpriteRenderer = selectionGameObject.AddComponent<SpriteRenderer>();
        selectionSpriteRenderer.sprite = cursorSprite;
        selectionSpriteRenderer.sortingOrder = -500;
        selectionSpriteRenderer.color = new Color(1, 1, 1, 0.25f);

#if UNITY_EDITOR
        selectionGameObject.name = "SelectionCursor";
#endif

        gridManagerReference.AddListener(OnFoundGridReference);

    }

    private void OnDestroy()
    {
        gridManagerReference.RemoveListener(OnFoundGridReference);
    }

    private void OnFoundGridReference(GameObject obj)
    {
        gridManager = obj.GetComponent<GridManager>();
    }

    private void OnGamePauze(bool state)
    {
        if (selectionGameObject != null)
        {
            selectionGameObject.gameObject.SetActive(state);
        }
    }

    private void OnMouseMove(Vector2 location)
    {
        lastMousePosition = location;

        if (gridManager == null || frozen)
        {
            return;
        }

        mouseGridLocation = gridManager.Grid.WorldToCell(location);

        if (Vector3Int.Distance(mouseGridLocation, characterGridLocation) <= selectionTileDistance)
        {
            selectionGameObject.gameObject.SetActive(displaySelectionView);

            if (gridManager != null)
            {
                selectionGameObject.transform.position = (Vector2)gridManager.Grid.CellToWorld(mouseGridLocation) + gridOffset;
                currentSelectionGridPosition = mouseGridLocation;
            }
        }
        else
        {
            selectionGameObject.gameObject.SetActive(false);
            currentSelectionGridPosition = characterForwardGridLocation;
        }
    }

    public void OnMove(Vector2 direction, float velocity)
    {
        lastMoveDirection = direction;

        if (frozen)
        {
            return;
        }

        if (gridManager != null)
        {
            characterGridLocation = gridManager.Grid.WorldToCell((Vector2)this.transform.position);

            if (direction != Vector2.zero)
            {
                characterForwardGridLocation = gridManager.Grid.WorldToCell(((Vector2)this.transform.position + Vector2.ClampMagnitude(direction, gridManager.Grid.cellSize.x)));
            }

            OnMouseMove(lastMousePosition += (direction * velocity) * Time.deltaTime);
        }
    }

    public Vector2 GetGridLookDirection()
    {
        if (Vector3Int.Distance(mouseGridLocation, characterGridLocation) <= selectionTileDistance)
        {
            return ((Vector3)lastMousePosition - (Vector3)this.transform.position).normalized;
        }
        else
        {
            return lastMoveDirection;
        }

    }

    public Vector3Int GetGridSelectionPosition()
    {
        return currentSelectionGridPosition;
    }

    public Vector2 GetGridWorldSelectionPosition()
    {
        if (gridManager == null)
        {
            return this.transform.position;
        }

        return (Vector2)gridManager.Grid.CellToWorld(GetGridSelectionPosition()) + gridOffset;
    }

    public bool IsStandingOnSelectedTile()
    {
        return currentSelectionGridPosition == characterGridLocation;
    }

    public void Display(bool display)
    {
        displaySelectionView = display;

        selectionGameObject.gameObject.SetActive(display);
    }

    public void SetFrozen(bool frozen)
    {
        this.frozen = frozen;

        if (!frozen)
        {
            OnMouseMove(lastMousePosition);
        }
    }
}
