using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Lowscope.Saving;

/// <summary>
/// Responsible for sending time events to listeners.
/// Handles the current time of the game.
/// </summary>
[AddComponentMenu("Farming Kit/Systems/Time System")]
public class TimeSystem : GameSystem, ISaveable
{
    // Happens based on the given tick rate of this system
    [SerializeField]
    private TimeEvent timeTickEvent;

    // Happens at start, and when it is a new day, useful to minimize the amount of event calls.
    [SerializeField]
    private TimeEvent timeDayEvent;

    [SerializeField]
    private BoolEvent pauzeEvent;

    [SerializeField, Range(1, 9999)]
    private int year = 2000;

    [SerializeField, Range(1, 12)]
    private int month = 5;

    [SerializeField, Range(1, 31)]
    private int day = 1;

    [SerializeField, Range(0, 23)]
    private int hour = 8;

    [SerializeField]
    private int minutesPerTick = 1;

    [SerializeField, Tooltip("Offset when the new day event happens. Setting this to 6 means that it will happen on 6 AM")]
    private IntVariable newDayHourOffset;

    public DateTime startTime;
    public DateTime currentTime;
    public TimeSpan ElapsedTime => currentTime - startTime;

    private bool isNewDay = false;
    private bool loadedSave = false;

    public override void OnLoadSystem()
    {
        if (!loadedSave)
        {
            startTime = new DateTime(year, month, day, hour, 0, 0);
            currentTime = startTime;
        }

        pauzeEvent?.AddListener(OnGamePauzed);
    }

    public void Start()
    {
        timeTickEvent?.Invoke(currentTime);
        timeDayEvent?.Invoke(currentTime);
    }

    public override void OnTick()
    {
        DateTime currentTimeCopy = currentTime;

        AddMinute(minutesPerTick);

        if (currentTimeCopy.DayOfWeek != currentTime.DayOfWeek)
        {
            if (newDayHourOffset.Value == 0)
            {
                timeDayEvent?.Invoke(currentTime);
            }
            else
            {
                isNewDay = true;
            }
        }

        if (isNewDay && currentTime.TimeOfDay.Hours >= newDayHourOffset.Value)
        {
            timeDayEvent?.Invoke(currentTime);
            isNewDay = false;
        }
    }

    private void OnGamePauzed(bool state)
    {
        Pauze(state);
    }

    /// <param name="year"> Specify a year between 1 and 9999</param>
    public void SetYear(int year)
    {
        currentTime = ChangeTime(year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute);
        timeTickEvent?.Invoke(currentTime);
    }

    /// <param name="month"> Specify a month between 1 and 12</param>
    public void SetMonth(int month)
    {
        currentTime = ChangeTime(currentTime.Year, month, currentTime.Day, currentTime.Hour, currentTime.Minute);
        timeTickEvent?.Invoke(currentTime);
    }

    /// <param name="day"> Specify a day between 1 and 31</param>
    public void SetDay(int day)
    {
        currentTime = ChangeTime(currentTime.Year, currentTime.Month, day, currentTime.Hour, currentTime.Minute);
        timeTickEvent?.Invoke(currentTime);
    }

    /// <param name="hour"> Specify a hour between 0 and 23. (0 to 12 = AM), (12 to 23 = PM)</param>
    public void SetHour(int hour)
    {
        currentTime = ChangeTime(currentTime.Year, currentTime.Month, currentTime.Day, hour, currentTime.Minute);
        timeTickEvent?.Invoke(currentTime);
    }

    /// <param name="minute"> Specify a minute between 0 and 59</param>
    public void SetMinute(int minute)
    {
        currentTime = ChangeTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, minute);
        timeTickEvent?.Invoke(currentTime);
    }

    public void AddYear(int amount)
    {
        currentTime = currentTime.AddYears(amount);
        timeTickEvent?.Invoke(currentTime);
    }

    public void AddMonth(int amount)
    {
        currentTime = currentTime.AddMonths(amount);
        timeTickEvent?.Invoke(currentTime);
    }

    public void AddDay(int amount)
    {
        currentTime = currentTime.AddDays(amount);
        timeTickEvent?.Invoke(currentTime);
    }

    public void AddHour(int amount)
    {
        currentTime = currentTime.AddHours(amount);
        timeTickEvent?.Invoke(currentTime);
    }

    public void AddMinute(int amount)
    {
        currentTime = currentTime.AddMinutes(amount);
        timeTickEvent?.Invoke(currentTime);
    }

    private DateTime ChangeTime(int year, int month, int day, int hours, int minutes)
    {
        return new DateTime(
            year,
            month,
            day,
            hours,
            minutes,
            0);
    }

    [System.Serializable]
    public struct SaveData
    {
        public int year;
        public int month;
        public int day;
        public int hour;
        public int minute;

        public bool isNewDay;

        public bool IsValidDate()
        {
            return (year + month + day + hour + minute) != 0;
        }
    }

    public string OnSave()
    {
        return JsonUtility.ToJson(new SaveData()
        {
            year = currentTime.Year,
            month = currentTime.Month,
            day = currentTime.Day,
            hour = currentTime.Hour,
            minute = currentTime.Minute,
            isNewDay = this.isNewDay
        });
    }

    public void OnLoad(string _data)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(_data);

        if (data.IsValidDate())
        {
            currentTime = new DateTime(data.year, data.month, data.day, data.hour, data.minute, 0);
            this.isNewDay = data.isNewDay;
            timeTickEvent?.Invoke(currentTime);
        }

        loadedSave = true;
    }

    public bool OnSaveCondition()
    {
        return true;
    }
}