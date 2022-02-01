using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

[System.Serializable]
public static class UserInterfacePriorityKeeper
{
    private static List<UserInterfaceNavigation> navigationInstances = new List<UserInterfaceNavigation>();

    public static void Add(UserInterfaceNavigation nav)
    {
        if (!navigationInstances.Contains(nav))
        {
            navigationInstances.Add(nav);
        }
    }

    public static void Remove(UserInterfaceNavigation nav)
    {
        if (navigationInstances.Contains(nav))
        {
            navigationInstances.Remove(nav);
        }
    }

    public static UserInterfaceNavigation GetLastInstance()
    {
        if (navigationInstances.Count > 0)
        {
            return navigationInstances.Last();
        }
        else return null;
    }

    public static int GetInstanceCount()
    {
        return navigationInstances.Count;
    }
}

public class UserInterfaceNavigation : MonoBehaviour
{
    [SerializeField]
    private GameEvent OnClick;

    [SerializeField]
    private Vector2Event OnMove;

    [SerializeField]
    private GameObject firstFocus;

    [SerializeField]
    private Button[] focussableObjects;

    private Vector2 lastInput;

    private EventSystem eventSystem;
    private GameObject currentFocus => eventSystem.currentSelectedGameObject;

    private GameObject lastFocus;
    private PointerEventData pointer;

    private void Awake()
    {
        focussableObjects = GetComponentsInChildren<Button>();

        eventSystem = EventSystem.current;
        pointer = new PointerEventData(eventSystem);
    }

    private void OnEnable()
    {
        UserInterfacePriorityKeeper.Add(this);

        FocusDefault();

        OnClick?.AddListener(OnMouseClick);
        OnMove?.AddListener(OnMoveAxis);
    }

    public void FocusDefault()
    {
        GameObject targetFocus;

        if (firstFocus == null)
        {
            targetFocus = focussableObjects[0].gameObject;
        }
        else
        {
            targetFocus = firstFocus;
        }

        
        eventSystem.SetSelectedGameObject(targetFocus);
        
        /*
        ExecuteEvents.Execute(targetFocus, pointer, ExecuteEvents.pointerEnterHandler);
        lastFocus = targetFocus;
        */
    }

    private void OnDisable()
    {
        UserInterfacePriorityKeeper.Remove(this);

        UserInterfacePriorityKeeper.GetLastInstance()?.FocusDefault();

        OnClick?.RemoveListener(OnMouseClick);
        OnMove?.RemoveListener(OnMoveAxis);
    }

    private void OnMouseClick()
    {
        for (int i = 0; i < focussableObjects.Length; i++)
        {
            if (currentFocus == focussableObjects[i])
            {
                return;
            }
        }

        eventSystem.SetSelectedGameObject(lastFocus ?? firstFocus);
    }

    private void OnMoveAxis(Vector2 axis)
    {
        if (lastInput == axis)
        {
            return;
        }

        lastInput = axis;

        if (lastFocus == currentFocus)
        {
            return;
        }

        if (lastFocus != null)
        {
            ExecuteEvents.Execute(lastFocus, pointer, ExecuteEvents.pointerExitHandler);
        }

        lastFocus = currentFocus;

        ExecuteEvents.Execute(currentFocus, pointer, ExecuteEvents.pointerEnterHandler);
    }
}