using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public struct TimePlayed
{
    public int seconds;
    public int minutes;
    public int hours;
    public int days;

    public static implicit operator TimePlayed(TimeSpan timeSpan)
    {
        return new TimePlayed() { seconds = timeSpan.Seconds, minutes = timeSpan.Minutes, hours = timeSpan.Hours, days = timeSpan.Days };
    }

    public static implicit operator TimeSpan(TimePlayed timePlayed)
    {
        return new TimeSpan(timePlayed.days, timePlayed.hours, timePlayed.minutes, timePlayed.seconds);
    }
}