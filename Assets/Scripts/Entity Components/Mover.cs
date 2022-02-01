using Lowscope.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Farming Kit/Entity Components/Movement/Mover")]
[RequireComponent(typeof(Rigidbody2D)), DisallowMultipleComponent()]
public class Mover : MonoBehaviour, ISaveable
{
    [SerializeField]
    private float speed;

    private List<IMove> IMoveInterfaces = new List<IMove>();
    private List<IFreezeMovement> IFreezeMovementInterfaces = new List<IFreezeMovement>();

    private Rigidbody2D rigidBody2D;

    private bool isMovementFrozen;
    public bool IsMovementFrozen { get { return isMovementFrozen; } }

    private void Awake()
    {
        if (rigidBody2D == null)
            rigidBody2D = GetComponent<Rigidbody2D>();

        GetComponentsInChildren<IMove>(true, IMoveInterfaces);
        GetComponentsInChildren<IFreezeMovement>(true, IFreezeMovementInterfaces);

        DispatchMoveEvent(Vector2.zero, 0);
    }

    public void Move(Vector2 direction)
    {
        if (isMovementFrozen)
        {
            return;
        }

        direction.Normalize();

        rigidBody2D.MovePosition((Vector2)this.transform.position + ((direction * speed) * Time.deltaTime));

        DispatchMoveEvent(direction, (direction.x == 0 && direction.y == 0) ? 0 : speed);
    }

    public void SetPosition(Vector2 position)
    {
        rigidBody2D.MovePosition(position);
        DispatchMoveEvent(Vector2.zero, 0);
    }
        
    public void FreezeMovement(bool state)
    {
        isMovementFrozen = state;

        DispatchMoveEvent(Vector2.zero, 0);
        DispatchFreezeEvent(state);
    }

    private void DispatchMoveEvent(Vector2 direction, float speed)
    {

        for (int i = 0; i < IMoveInterfaces.Count; i++)
        {
            IMoveInterfaces[i].OnMove(direction, speed);
        }
    }

    private void DispatchFreezeEvent(bool isFrozen)
    {


        for (int i = 0; i < IFreezeMovementInterfaces.Count; i++)
        {
            IFreezeMovementInterfaces[i].OnMovementFrozen(isFrozen);
        }
    }

    #region Saving

    private Vector2 lastSavedPosition;

    [System.Serializable]
    public struct SaveData
    {
        public Vector2 position;
    }

    public string OnSave()
    {
        lastSavedPosition = this.transform.position;

        return JsonUtility.ToJson(new SaveData()
        {
            position = lastSavedPosition
        });
    }

    public void OnLoad(string data)
    {
        SaveData saveData = JsonUtility.FromJson<SaveData>(data);

        Vector2 newPosition = saveData.position;

        this.transform.position = newPosition;

        if (rigidBody2D == null)
            rigidBody2D = GetComponent<Rigidbody2D>();

        if (rigidBody2D.bodyType != RigidbodyType2D.Static)
        {
            rigidBody2D.MovePosition(newPosition);
        }

        DispatchMoveEvent(Vector2.zero, 0);
        lastSavedPosition = newPosition;
    }

    public bool OnSaveCondition()
    {
        return lastSavedPosition != (Vector2)this.transform.position;
    }

    #endregion
}