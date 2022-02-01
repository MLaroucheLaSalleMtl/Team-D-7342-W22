using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

[AddComponentMenu("Farming Kit/Events/Weather Event Listener")]
public class WeatherEventListener : ScriptableEventListener<EWeather>
{
    [SerializeField]
    protected WeatherEvent eventObject;

    [SerializeField, HideInInspector]
    protected UnityEventWeather eventAction;

    [SerializeField]
    private UnityEvent OnSunnyWeather;

    [SerializeField]
    private UnityEvent OnNormalWeather;

    [SerializeField]
    private UnityEvent OnRainyWeather;

    [SerializeField]
    private UnityEvent OnSnowWeather;

    protected override ScriptableEvent<EWeather> ScriptableEvent
    {
        get
        {
            return eventObject;
        }
    }

    protected override UnityEvent<EWeather> Action
    {
        get
        {
            return eventAction;
        }
    }

    private void Awake()
    {
        eventAction?.AddListener(OnWeatherChanged);
    }

    private void OnWeatherChanged(EWeather state)
    {
        switch (state)
        {
            case EWeather.Sunny:
                OnSunnyWeather.Invoke();
                break;
            case EWeather.Normal:
                OnNormalWeather.Invoke();
                break;
            case EWeather.Rainy:
                OnRainyWeather.Invoke();
                break;
            case EWeather.Snow:
                OnSnowWeather.Invoke();
                break;
            default:
                break;
        }
    }
}
