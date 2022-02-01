using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;

/// <summary>
/// Used to define the current aim direction.
/// Player mouse cursor aiming tools will interact with this component.
/// </summary>
[AddComponentMenu("Farming Kit/Entity Components/Movement/Aimer")]
public class Aimer : MonoBehaviour, IMove, ISaveable
{
    private List<IAim> IAimInterfaces = new List<IAim>();

    private Vector2 lastAimDirection;

    private void Awake()
    {
        GetComponentsInChildren<IAim>(true, IAimInterfaces);
    }

    public void OnMove(Vector2 direction, float velocity)
    {
        if (direction != Vector2.zero)
        {
            lastAimDirection = direction;
            isDirty = true;
        }
    }

    public Vector2 GetAimDirection()
    {
        return lastAimDirection;
    }

    public Vector2 GetAimPosition()
    {
        return (Vector2)transform.position + (GetAimDirection() * 0.10f);
    }

    public void LookAt(Vector2 target)
    {
        Vector2 lookVector = (target - (Vector2)this.transform.position);

        if (Mathf.Abs(lookVector.x) > Mathf.Abs(lookVector.y))
        {
            lookVector.y = 0;

            if (lookVector.x > 0)
            {
                lookVector.x = 1;
            }
            else
            {
                lookVector.x = -1;
            }
        }
        else
        {
            lookVector.x = 0;

            if (lookVector.y > 0)
            {
                lookVector.y = 1;
            }
            else
            {
                lookVector.y = -1;
            }
        }

        SetAimDirection(lookVector);
    }

    public void SetAimDirection(Vector2 direction)
    {
        isDirty = true;
        IAimInterfaces.ForEach((_interface) => _interface.OnAim(direction));
        lastAimDirection = direction;
    }

    #region Saving

    [System.Serializable]
    public struct RuntimeData
    {
        public Vector2 aimDirection;
    }

    public string OnSave()
    {
        return JsonUtility.ToJson(new RuntimeData() { aimDirection = lastAimDirection });
    }

    public void OnLoad(string data)
    {
        RuntimeData getData = JsonUtility.FromJson<RuntimeData>(data);

        lastAimDirection = getData.aimDirection;
        SetAimDirection(getData.aimDirection);
        IAimInterfaces.ForEach((_interface) => _interface.OnAim(getData.aimDirection));
    }

    private bool isDirty = false;

    public bool OnSaveCondition()
    {
        if (isDirty)
        {
            isDirty = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}