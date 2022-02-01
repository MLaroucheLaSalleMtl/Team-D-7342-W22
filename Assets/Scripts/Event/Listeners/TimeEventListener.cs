using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[AddComponentMenu("Farming Kit/Events/Time Event Listener")]
public class TimeEventListener : ScriptableEventListener<DateTime>
{
    [SerializeField]
    private TimeEvent eventObject;

    private UnityEventDate eventAction = new UnityEventDate();

    private void Awake()
    {
        eventAction?.AddListener(OnTimeTick);
    }

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
    public class TimeToggleEvents
    {
        [System.Serializable]
        public struct Time
        {
            public int hour;
            public int minutes;

            private bool initialized;

            private TimeSpan span;

            public TimeSpan Span
            {
                get
                {
                    if (!initialized)
                    {
                        span = new TimeSpan(hour, minutes, 0);
                        initialized = true;
                    }

                    return span;
                }
            }
        }

        public Time start;
        public Time end;

        public UnityEvent WithinTime;
        public UnityEvent OutsideTime;

        private bool isWithinTime;
        private bool initializationCheck;

        public void Evaluate(DateTime dateTime)
        {
            TimeSpan now = dateTime.TimeOfDay;

            bool currentState = isWithinTime;

            if (start.Span <= end.Span)
            {
                // start and stop times are in the same day
                if (now >= start.Span && now <= end.Span)
                {
                    isWithinTime = true;
                }
                else
                {
                    isWithinTime = false;
                }
            }
            else
            {
                // start and stop times are in different days
                if (now >= start.Span || now <= end.Span)
                {
                    isWithinTime = true;
                }
                else
                {
                    isWithinTime = false;
                }
            }

            if (currentState != isWithinTime || !initializationCheck)
            {
                if (isWithinTime)
                {
                    WithinTime.Invoke();
                }
                else
                {
                    OutsideTime.Invoke();
                }

                initializationCheck = true;
            }
        }
    }

    [System.Serializable]
    public class ValueEvent
    {
        public AnimationCurve valueRange = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0f), new Keyframe(1, 1f) });

        public UnityEventFloat onTimeChangeValue;
    }

    [System.Serializable]
    public class StringEvent
    {
        [SerializeField]
        public ETextOutput outputStyle;

        public UnityEventString onTimeChangeString;

        public enum ETextOutput
        {
            TimeAM,
            Time24,
            DateValues,
            DateText
        }

    }

    public List<TimeToggleEvents> timeEvents;
    public ValueEvent valueEvent;
    public StringEvent stringEvent;

    private readonly float maxMinutes = 1380;

    private void OnTimeTick(DateTime dateTime)
    {
        valueEvent.onTimeChangeValue.Invoke(valueEvent.valueRange.Evaluate((float)dateTime.TimeOfDay.TotalMinutes / maxMinutes));
        stringEvent.onTimeChangeString.Invoke(DateTimeToStringOutput(dateTime));

        for (int i = 0; i < timeEvents.Count; i++)
        {
            timeEvents[i].Evaluate(dateTime);
        }
    }

    private string DateTimeToStringOutput(DateTime dateTime)
    {
        switch (stringEvent.outputStyle)
        {
            case StringEvent.ETextOutput.TimeAM:
                return dateTime.ToString("hh:mm tt", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            case StringEvent.ETextOutput.Time24:
                return dateTime.ToString("HH:mm");

            default:
                break;
        }

        return "";
    }
}