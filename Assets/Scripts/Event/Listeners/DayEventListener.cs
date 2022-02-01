using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Lowscope.Saving;

[AddComponentMenu("Farming Kit/Events/Day Event Listener")]
public class DayEventListener : ScriptableEventListener<DateTime>, ISaveable
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

    [System.Serializable]
    private class OnDate
    {
        public Date date;
        public UnityEvent OnDateEvent;
        public bool allowEnableLater;
    }

    [SerializeField]
    private UnityEvent OnOneDayPassed;

    [SerializeField]
    private UnityEventInt OnMultipleDaysPassed;

    [SerializeField]
    private OnDate onDate;

    private bool hasOnDateBeenInvoked;
    private DateTime? lastSeen;

    private void Awake()
    {
        eventAction?.AddListener(OnDayTick);
    }

    private void OnDestroy()
    {
        eventAction?.RemoveListener(OnDayTick);
    }

    private void OnDayTick(DateTime dateTime)
    {
        dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

        if (lastSeen == null)
        {
            lastSeen = dateTime;
            Debug.Log("First time setting DateTime");
        }

        int totalDaysPassed = (int)(dateTime - (DateTime)lastSeen).TotalDays;

        if (totalDaysPassed > 0)
        {
            if (totalDaysPassed == 1)
            {
                OnOneDayPassed.Invoke();
            }
            else
            {
                OnMultipleDaysPassed.Invoke(totalDaysPassed);
            }
        }

        if (!hasOnDateBeenInvoked)
        {
            bool invokeEvent = false;

            if (onDate.date.Compare(dateTime))
            {
                invokeEvent = true;
            }
            else
            {
                if (onDate.allowEnableLater && onDate.date.ToDateTime() < dateTime)
                {
                    invokeEvent = true;
                }
            }

            if (invokeEvent)
            {
                invokeEvent = true;
            }
        }

        lastSeen = dateTime;

        canSave = true;
    }

    [System.Serializable]
    public struct SaveData
    {
        public bool hasOnDateBeenInvoked;
        public Date lastSeen;
    }

    private bool canSave;

    public void OnLoad(string data)
    {
        if (!string.IsNullOrEmpty(data))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(data);

            hasOnDateBeenInvoked = saveData.hasOnDateBeenInvoked;
            lastSeen = saveData.lastSeen.ToDateTime();
        }
    }

    public string OnSave()
    {
        DateTime lastSeenDate = ((DateTime)lastSeen);

        return JsonUtility.ToJson(new SaveData()
        {
            lastSeen = new Date()
            {
                year = lastSeenDate.Year,
                day = lastSeenDate.Day,
                month = lastSeenDate.Month
            },
            hasOnDateBeenInvoked = this.hasOnDateBeenInvoked
        });
    }

    public bool OnSaveCondition()
    {
        return canSave;
    }
}