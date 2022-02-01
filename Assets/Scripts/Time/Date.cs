using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Date
{
    public int year;
    public int day;
    public int month;

    public DateTime ToDateTime()
    {
        if (year == 0 && day == 0 && month == 0)
        {
            return default(DateTime);
        }

        return new DateTime(year, month, day);
    }

    public bool Compare(DateTime dateTime)
    {
        return dateTime.Year == year && dateTime.Day == day && dateTime.Month == month;
    }
}