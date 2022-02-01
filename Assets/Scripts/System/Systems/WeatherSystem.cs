using Lowscope.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the weather state, generates a new weather type per day.
/// Invokes OnWeatherChange events to objects.
/// </summary>

[AddComponentMenu("Farming Kit/Systems/Weather System")]
public class WeatherSystem : GameSystem, ISaveable
{
    [SerializeField]
    private TimeEvent onDayTick;

    [SerializeField]
    private WeatherEvent onWeather;

    [System.Serializable]
    public class WeightedWeather
    {
        public EWeather type;
        public int weight;
    }

    public List<WeightedWeather> weatherTypes = new List<WeightedWeather>();

    private EWeather currentWeather;

    private int baseSeed = 0;
    private int dayIndex = 0;

    private float dayOfYear;

    private System.Random seed;

    public override void OnLoadSystem()
    {
        if (baseSeed == 0)
        {
            baseSeed = DateTime.Now.GetHashCode();

            seed = new System.Random(baseSeed);
        }

        onDayTick.AddListener(OnNewDay);
    }

    private void OnNewDay(DateTime time)
    {
        // Execute if the current time doesn't match the given day time
        // In case the game loads a save game, we don't want to set the weather again on the same day.
        if (time.DayOfYear != dayOfYear)
        {
            dayOfYear = time.DayOfYear;
            dayIndex++;

            var cumulativeWeight = 0;
            foreach (var weatherType in weatherTypes) cumulativeWeight += weatherType.weight;

            var randomWeight = seed.Next(1, cumulativeWeight + 1);

            foreach (var weatherType in weatherTypes)
            {
                randomWeight -= weatherType.weight;

                if (randomWeight <= 0)
                {
                    currentWeather = weatherType.type;

                    break;
                }
            }
            
            // TODO: Create a season system, that swaps out the option for rain with snow.
        }

        onWeather.Invoke(currentWeather);

        isSaveable = true;
    }

    private bool isSaveable;

    public struct SaveData
    {
        public int baseSeed;
        public int dayIndex;
        public float dayOfYear;
        public int currentWeather;
    }

    public string OnSave()
    {
        isSaveable = false;

        return JsonUtility.ToJson(new SaveData()
        {
            baseSeed = this.baseSeed,
            dayIndex = this.dayIndex,
            dayOfYear = this.dayOfYear,
            currentWeather = (int)this.currentWeather
        });
    }

    public void OnLoad(string data)
    {
        SaveData saveData = JsonUtility.FromJson<SaveData>(data);

        this.baseSeed = saveData.baseSeed;
        this.seed = new System.Random(saveData.baseSeed);

        this.dayIndex = saveData.dayIndex;
        this.dayOfYear = saveData.dayOfYear;
        this.currentWeather = (EWeather) saveData.currentWeather;
    }

    public bool OnSaveCondition()
    {
        return isSaveable;
    }
}
