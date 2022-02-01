using Lowscope.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour, IKillable, ISaveable, ITilemapListener
{
    [System.Serializable]
    public class DaySprites
    {
        public Sprite sprite;
        public int day;
    }

    [System.Serializable]
    public struct SaveData
    {
        public int day;
        public int dryDays;
        public bool wasWet;
        public string lastDate;
    }

    [Header("References")]
    [SerializeField] private ScriptableReference gridManagerReference;
    [SerializeField] private TimeEvent onTimeDayEvent;
    [SerializeField] private Health health;
    [SerializeField] private ItemDropper itemDropper;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Crop configuration")]
    [SerializeField] private string groundWetTilemapName;
    [SerializeField] private IntVariable timeDayOffset;
    [SerializeField] private int surviveDaysWithoutWater;
    [SerializeField] private int harvestDayStart;
    [SerializeField] private int harvestDayEnd;
    [SerializeField] private DaySprites[] daySprites;

    private GridManager gridManager;

    private int day = 0;
    private int dryDays;
    private bool wasWet;
    private DateTime lastDate = default;

    private Dictionary<int, Sprite> indexedDaySprites = new Dictionary<int, Sprite>();

    private void Awake()
    {
        var getGridManager = gridManagerReference.Reference?.GetComponent<GridManager>();
        gridManager = getGridManager;

        IndexDaySprites();
        onTimeDayEvent.AddListener(OnTimeDayEvent);
    }

    private void Start()
    {
        gridManager.SubscribeToGridChanges(this, this.transform.position);
    }

    private void OnDestroy()
    {
        if (gridManager != null)
            gridManager.UnSubscribeToGridChanges(this, this.transform.position);

        onTimeDayEvent.RemoveListener(OnTimeDayEvent);
    }

    private void IndexDaySprites()
    {
        int lowIndex;
        int highIndex = harvestDayEnd;

        int daySpriteCount = daySprites.Length;
        for (int i = daySpriteCount - 1; i >= 0; i--)
        {
            lowIndex = daySprites[i].day;
            Sprite sprite = daySprites[i].sprite;

            for (int i2 = highIndex; i2 >= lowIndex; i2--)
            {
                indexedDaySprites.Add(i2, sprite);
            }

            highIndex = lowIndex - 1;
        }
    }

    private void OnTimeDayEvent(DateTime dateTime)
    {
        // If it is still the same day, the plant will stay the same.
        if ((dateTime.Day == lastDate.Day || dateTime.Hour < timeDayOffset.Value) && lastDate != default)
        {
            UpdateSprite();
            return;
        }
        else
        {
            bool wasDryDay = false;

            // Don't check for a dry day if initially planted
            if (lastDate != default)
            {
                if (!gridManager.ContainsMapAtLocation(groundWetTilemapName, transform.position))
                {
                    // Ensure growth if it was once wet.
                    if (!wasWet)
                    {
                        dryDays++;
                        wasDryDay = true;
                    }
                    else
                    {
                        wasWet = false;
                    }

                    if (dryDays > surviveDaysWithoutWater)
                    {
                        this.gameObject.SetActive(false);
                        return;
                    }
                }
                else
                {
                    wasWet = true;
                    dryDays = 0;
                }
            }

            lastDate = dateTime;

            // Stall plant progress if it was a dry day.
            if (!wasDryDay)
            {
                day++;
            }

            // Plant becomes damageable once we can harvest it
            if (day >= harvestDayStart)
            {
                health.SetInvulnerable(false);
            }

            // Plant dissapears if it exceeds the harvest day
            if (day >= harvestDayEnd)
            {
                this.gameObject.SetActive(false);
                return;
            }
        }

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        Sprite sprite = null;
        if (indexedDaySprites.TryGetValue(day, out sprite))
        {
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogFormat("Have not found a sprite for day : {0}", day);
        }
    }

    // When object gets harvested
    public void OnDeath(Health health)
    {
        itemDropper.Drop(this.transform.position);
        this.gameObject.SetActive(false);
    }

    public string OnSave()
    {
        return JsonUtility.ToJson(new SaveData()
        {
            day = this.day,
            dryDays = this.dryDays,
            lastDate = this.lastDate.ToString(),
            wasWet = this.wasWet
        });
    }

    public void OnLoad(string data)
    {
        SaveData saveData = JsonUtility.FromJson<SaveData>(data);
        this.day = saveData.day;
        this.dryDays = saveData.dryDays;
        DateTime.TryParse(saveData.lastDate, out this.lastDate);
    }

    public bool OnSaveCondition()
    {
        return true;
    }

    public void OnAddedTile(string tilemapName)
    {
        if (tilemapName == groundWetTilemapName)
        {
            wasWet = true;
            dryDays = 0;
        }
    }

    public void OnRemovedTile(string tilemapName)
    {

    }
}
