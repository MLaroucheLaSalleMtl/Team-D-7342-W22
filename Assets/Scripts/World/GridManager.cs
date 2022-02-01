using Lowscope.Saving;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Responsible for managing the grids in a scene.
/// Is used to access grids on specific locations.
/// Saveable component, all tile actions are saved.
/// </summary>
[RequireComponent(typeof(Grid))]
public class GridManager : MonoBehaviour, ISaveable
{
    // Data container for all manipulated tiles
    [System.Serializable]
    public struct TileManipulationAction
    {
        // All tiles inherit from a custom class that provides an GUID
        // This class is named ScriptableTileBase.
        public string Guid;
        public Vector3Int Location;
        public string Tag;
    }

    private Grid grid;
    public Grid Grid
    {
        get
        {
            if (grid == null)
            {
                grid = GetComponent<Grid>();
                return grid;
            }

            return grid;
        }
    }

    private Dictionary<string, Tilemap> tileMaps = new Dictionary<string, Tilemap>();

    [SerializeField]
    private ScriptableTileBase wateredDirtTile;

    [SerializeField]
    private ScriptableTileBase dirtTile;

    [SerializeField]
    private ScriptableTileBase dirtHoleTile;

    [SerializeField]
    private Tilemap wateredDirtTileMap;

    public Tilemap WateredDirtTileMap { get { return wateredDirtTileMap; } }

    [SerializeField]
    private Tilemap dirtTileMap;

    public Tilemap DirtTileMap { get { return dirtTileMap; } }

    [SerializeField]
    private Tilemap dirtHoleTileMap;

    public Tilemap DirtHoleTileMap { get { return dirtHoleTileMap; } }

    [SerializeField]
    private Tilemap waterTileMap;

    public Tilemap WaterTileMap { get { return waterTileMap; } }

    private Dictionary<Vector3Int, List<ITilemapListener>> listeners = new Dictionary<Vector3Int, List<ITilemapListener>>();

    private bool initialized = false;

    private void Awake()
    {
        Initialize();
    }

    private void EmitGridTilemapAdd(Vector3Int position, string tilemapName)
    {
        List<ITilemapListener> list;

        if (listeners.TryGetValue(position, out list))
        {
            int listenerCount = list.Count;
            for (int i = 0; i < listenerCount; i++)
            {
                list[i]?.OnAddedTile(tilemapName);
            }
        }
    }

    private void EmitGridTilemapRemove(Vector3Int position, string tilemapName)
    {
        List<ITilemapListener> list;
        if (listeners.TryGetValue(position, out list))
        {
            int listenerCount = list.Count;
            for (int i = 0; i < listenerCount; i++)
            {
                list[i]?.OnRemovedTile(tilemapName);
            }
        }
    }

    public void SubscribeToGridChanges(ITilemapListener listener, Vector3 pos)
    {
        pos.z = 0;
        Vector3Int gridPos = GetGridLocation(pos);
        List<ITilemapListener> list;

        if (listeners.TryGetValue(gridPos, out list))
        {
            if (!list.Contains(listener))
            {
                list.Add(listener);
            }
        }
        else
        {
            listeners.Add(gridPos, new List<ITilemapListener>() { listener });
        }
    }

    public void UnSubscribeToGridChanges(ITilemapListener listener, Vector3 pos)
    {
        Vector3Int gridPos = GetGridLocation(pos);
        List<ITilemapListener> list;
        if (listeners.TryGetValue(gridPos, out list))
        {
            list.Remove(listener);
        }
    }

    private void Initialize()
    {
        if (!initialized)
        {
            grid = GetComponent<Grid>();

            Tilemap[] getTileMaps = GetComponentsInChildren<Tilemap>();

            for (int i = 0; i < getTileMaps.Length; i++)
            {
                tileMaps.Add(getTileMaps[i].name, getTileMaps[i]);
            }

            initialized = true;
        }
    }

    public void ClearTileMapData(Tilemap tileMap)
    {
        string tileMapName = tileMap.name;

        for (int i = saveData.actions.Count - 1; i >= 0; i--)
        {
            // TODO: Make sure this becomes optimized by providing an more efficient way of comparing.
            if (saveData.actions[i].Tag == tileMapName)
            {
                saveData.actions.RemoveAt(i);
            }
        }

        tileMap.ClearAllTiles();

        isDirty = true;
    }

    public int GetGridLocationTileCount(Vector3Int location)
    {
        int tiles = 0;

        foreach (Tilemap item in tileMaps.Values)
        {
            if (item.HasTile(location))
            {
                tiles++;
            }
        }

        return tiles;
    }

    public Tilemap GetTilemap(string name)
    {
        Tilemap tileMap;

        tileMaps.TryGetValue(name, out tileMap);

        if (tileMap != null)
        {
            return tileMap;
        }
        else
        {
            Debug.Log($"Failed attempt to get tilemap with name: {name}");
        }

        return null;
    }

    public Vector3Int GetGridLocation(Vector3 position)
    {
        return Grid.WorldToCell(position);
    }

    public Vector3 GetWorldLocation(Vector3Int position)
    {
        return Grid.CellToWorld(position) + grid.cellSize * 0.5f;
    }

    public bool ContainsMapAtLocation(string tileMapName, Vector3Int location)
    {
        Tilemap tileMap;

        if (tileMaps.TryGetValue(tileMapName, out tileMap))
        {
            if (tileMap.HasTile(location))
            {
                return true;
            }
        }

        return false;
    }

    public bool ContainsMapsAtLocation(string[] tileMapNames, Vector3Int location)
    {
        for (int i = 0; i < tileMapNames.Length; i++)
        {
            Tilemap tileMap;

            if (tileMaps.TryGetValue(tileMapNames[i], out tileMap))
            {
                if (tileMap.HasTile(location))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool ContainsMapAtLocation(string tileMapName, Vector3 worldPosition)
    {
        return ContainsMapAtLocation(tileMapName, GetGridLocation(worldPosition));
    }

    public bool ContainsMapsAtLocation(string[] tileMapNames, Vector3 worldPosition)
    {
        return ContainsMapsAtLocation(tileMapNames, GetGridLocation(worldPosition));
    }

    public ScriptableTileBase GetTile(Vector3Int location, string tilemapName)
    {
        Tilemap tileMap;

        tileMaps.TryGetValue(tilemapName, out tileMap);

        if (tileMap != null)
        {
            return tileMap.GetTile(location) as ScriptableTileBase;
        }
        else
        {
            Debug.Log($"Failed attempt to get tilemap with name: {tilemapName}");
        }

        return null;
    }

    public void SetTile(Vector3Int location, string tilemapName, ScriptableTileBase tileBase)
    {
        isDirty = true;

        Tilemap tileMap;

        tileMaps.TryGetValue(tilemapName, out tileMap);

        if (tileMap != null)
        {
            tileMap.SetTile(location, tileBase);

            EmitGridTilemapAdd(location, tilemapName);

            saveData.actions.Add(new TileManipulationAction()
            {
                Guid = tileBase.GetGuid(),
                Location = location,
                Tag = tilemapName
            });
        }
        else
        {
            Debug.Log($"Failed attempt to set tilemap with name: {tilemapName}");
        }
    }

    public bool HasDirt(Vector3Int position)
    {
        return dirtTileMap != null && dirtTileMap.GetTile(position) != null;
    }

    public bool HasDirtHole(Vector3Int position)
    {
        return dirtHoleTileMap != null && dirtHoleTileMap.GetTile(position) != null;
    }

    public bool HasWateredDirt(Vector3Int position)
    {
        return wateredDirtTileMap != null && wateredDirtTileMap.GetTile(position) != null;
    }

    public bool HasWater(Vector3Int position)
    {
        return waterTileMap != null && waterTileMap.GetTile(position) != null;
    }

    public void SetDirtTile(Vector3Int location)
    {
        if (dirtTileMap != null)
        {
            SetTile(location, dirtTileMap.name, dirtTile);
        }
    }

    public void SetWateredDirtTile(Vector3Int location)
    {
        if (wateredDirtTileMap != null)
        {
            SetTile(location, wateredDirtTileMap.name, wateredDirtTile);
        }
    }

    public void SetDirtHoleTile(Vector3Int location)
    {
        if (dirtHoleTileMap != null)
        {
            SetTile(location, dirtHoleTileMap.name, dirtHoleTile);
        }
    }

    #region Saving

    [System.Serializable]
    public class SaveData
    {
        public List<TileManipulationAction> actions = new List<TileManipulationAction>();
    }

    private SaveData saveData = new SaveData();

    public string OnSave()
    {
        return JsonUtility.ToJson(saveData);
    }

    public void OnLoad(string data)
    {
        Initialize();

        SaveData saveData = JsonUtility.FromJson<SaveData>(data);

        if (saveData != null)
        {
            for (int i = 0; i < saveData.actions.Count; i++)
            {
                ScriptableTileBase tileBase = ScriptableAssetDatabase.GetAsset(saveData.actions[i].Guid) as ScriptableTileBase;

                if (tileBase != null)
                {
                    SetTile(saveData.actions[i].Location, saveData.actions[i].Tag, tileBase);
                }
                else
                {
                    Debug.Log("Tried to obtain null tilebase data");
                }
            }
        }
    }

    private bool isDirty = false;

    public bool OnSaveCondition()
    {
        if (isDirty)
        {
            isDirty = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}
