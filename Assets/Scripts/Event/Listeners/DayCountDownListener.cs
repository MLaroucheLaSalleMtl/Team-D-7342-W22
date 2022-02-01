using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Lowscope.Saving;

[AddComponentMenu("Farming Kit/Events/Day Countdown Listener")]
public class DayCountDownListener : ScriptableEventListener<DateTime>, ISaveable
{
    [SerializeField]
    private TimeEvent eventObject;

    private UnityEventDate eventAction = new UnityEventDate();

    protected override ScriptableEvent<DateTime> ScriptableEvent
    {
        get
        {
            return eventObject;
        }
    }

    protected override UnityEvent<DateTime> Action
    {
        get
        {
            return eventAction;
        }
    }

    /// <summary>
    /// These events get invoked when the daycount is at a specific number
    /// </summary>
    [System.Serializable]
    private class DayCountEvents
    {
        [Tooltip("At what count (x: min, y: max) should this event be fired?")]
        public Vector2Int count;
        public UnityEvent onCountEvent;
    }

    [SerializeField]
    private int totalDays;

    [SerializeField]
    public UnityEvent onCountEvent;

    [SerializeField]
    private DayCountEvents[] dayCountEvents;

    [SerializeField, Tooltip("Invoke events if the counter has been frozen for a certain amount of time. " +
        "Freezing the counter happens when you call FreezeCountDown() on this component.")]
    private DayCountEvents[] frozenCountEvents;

    private DateTime? lastSeen = null;

    private int currentDayAmount;
    private int frozenDayAmount;
    private bool isFrozen;
    private bool firstInitialization;

    private bool loadedSave = false;

    /// <summary>
    /// Removes a day from the counter, and invokes the event if possible
    /// Reason for the returning is for saving.
    /// </summary>
    /// <returns> Has the event attached to the Day Counter been invoked? </returns>

    private void Start()
    {
        eventObject?.AddListener(OnDayTick);
    }

    private void OnDestroy()
    {
        eventObject?.RemoveListener(OnDayTick);
    }

    private void OnDayTick(DateTime dateTime)
    {
        dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

        if (lastSeen == null)
        {
            lastSeen = dateTime;
            currentDayAmount = totalDays;
            firstInitialization = false;
        }
        else
        {
            // If plant was loaded on same day, nothing happens.
            if (lastSeen == dateTime)
            {
                return;
            }
        }

        if (isFrozen && !firstInitialization)
        {
            lastSeen = dateTime;
            frozenDayAmount++;

            for (int i = 0; i < frozenCountEvents.Length; i++)
            {
                if (currentDayAmount >= frozenCountEvents[i].count.x && currentDayAmount <= frozenCountEvents[i].count.y)
                {
                    frozenCountEvents[i].onCountEvent.Invoke();
                }
            }

            return;
        }

        currentDayAmount -= (int)(dateTime - (DateTime)lastSeen).TotalDays;
        lastSeen = dateTime;

        for (int i = 0; i < dayCountEvents.Length; i++)
        {
            if (currentDayAmount >= dayCountEvents[i].count.x && currentDayAmount <= dayCountEvents[i].count.y)
            {
                dayCountEvents[i].onCountEvent.Invoke();
            }
        }

        onCountEvent.Invoke();

        canSave = true;
    }

    public void FreezeCountDown(bool setFrozen)
    {
        isFrozen = setFrozen;
    }

    [System.Serializable]
    public struct SaveData
    {
        public int currentDayAmount;
        public int frozenDayAmount;
        public bool hasBeenInvoked;
        public Date lastSeenDate;
    }

    private bool canSave;

    public void OnLoad(string data)
    {
        SaveData saveData = JsonUtility.FromJson<SaveData>(data);

        currentDayAmount = saveData.currentDayAmount;
        frozenDayAmount = saveData.frozenDayAmount;

        lastSeen = (DateTime)saveData.lastSeenDate.ToDateTime();
    }

    public string OnSave()
    {
        return JsonUtility.ToJson(new SaveData()
        {
            currentDayAmount = this.currentDayAmount,
            frozenDayAmount = this.frozenDayAmount,
            lastSeenDate = new Date()
            {
                year = lastSeen.Value.Year,
                month = lastSeen.Value.Month,
                day = lastSeen.Value.Day
            }
        });
    }

    public bool OnSaveCondition()
    {
        return canSave;
    }
}