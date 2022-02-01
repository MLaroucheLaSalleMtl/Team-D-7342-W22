using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// Handles input events
/// </summary>
[AddComponentMenu("Farming Kit/Systems/Input System")]
public class InputSystem : GameSystem
{
    [System.Serializable]
    public class Events
    {
        public GameEvent pauze;
        public Vector2Event movement;
        public IntEvent scroll;
        public IntEvent numeric;
        public Vector2Event leftMouseClick;
        public Vector2Event mouseMovement;
        public GameEvent accept;
        public Vector2Event rightMouseClick;
    }

    [SerializeField]
    private Events events;

    [SerializeField]
    private BoolEvent pauzeEvent;

    [SerializeField]
    private bool ignoreMouseWhenOverlappingUI;

    [System.NonSerialized]
    private bool gamePauzed;

    [System.NonSerialized]
    private Vector3 lastMousePosition;

    [System.NonSerialized]
    private Vector2 lastInput;

    [System.NonSerialized]
    private bool isMoving;

    private new Camera camera;
    private EventSystem eventSystem;

    public override void OnLoadSystem()
    {
        pauzeEvent?.AddListener(OnGamePauzed);
    }

    private void OnGamePauzed(bool state)
    {
        gamePauzed = state;

        if (state)
        {
            events.movement?.Invoke(Vector2.zero);
        }
    }

    public override void OnFixedTick()
    {
        if (!gamePauzed)
        {
            Vector2 movementVector;
            movementVector.x = Input.GetAxisRaw("Horizontal");
            movementVector.y = Input.GetAxisRaw("Vertical");

            if (movementVector.x != 0f || movementVector.y != 0f)
            {
                isMoving = true;
                events.movement?.Invoke(movementVector);
                UpdateMousePosition();
            }
            else
            {
                if (isMoving && movementVector.x == 0 && movementVector.y == 0)
                {
                    isMoving = false;
                    events.movement?.Invoke(movementVector);
                }
            }
        }
    }

    public override void OnTick()
    {
        if (!gamePauzed)
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                float mouseScrollDelta = Input.mouseScrollDelta.y;

                if (mouseScrollDelta > 0)
                {
                    events.scroll.Invoke(1);
                }
                else
                {
                    if (mouseScrollDelta < 0)
                    {
                        events.scroll.Invoke(-1);
                    }
                }
            }

        }

        if (camera == null)
        {
            foreach (Camera item in Camera.allCameras)
            {
                if (item != null)
                {
                    camera = item;
                }
            }
        }
        else
        {
            UpdateMousePosition();
        }

        if (Input.anyKey)
        {
            if (!gamePauzed)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (Input.GetKeyDown(i.ToString()))
                    {
                        if (i == 0)
                        {
                            events.numeric.Invoke(10);
                            break;
                        }

                        events.numeric.Invoke(i - 1);
                        break;
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (!ignoreMouseWhenOverlappingUI || !EventSystem.current.IsPointerOverGameObject())
                    {
                        events.leftMouseClick?.Invoke(camera.ScreenToWorldPoint(lastMousePosition));
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if (!ignoreMouseWhenOverlappingUI || !EventSystem.current.IsPointerOverGameObject())
                    {
                        events.rightMouseClick?.Invoke(camera.ScreenToWorldPoint(lastMousePosition));
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                events.pauze?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                events.accept?.Invoke();
            }

        }
    }

    private void UpdateMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0;

        if (lastMousePosition != mousePosition)
        {
            Vector3 mouseWorldPoint = camera.ScreenToWorldPoint(mousePosition);
            events.mouseMovement?.Invoke(mouseWorldPoint);
            lastMousePosition = mousePosition;
        }
    }
}
