using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using System;
using Lowscope.Saving;

/// <summary>
/// Notifies listeners to save the game.
/// Obtains saved data from callback, and writes it to a file.
/// </summary>

[AddComponentMenu("Farming Kit/Systems/Save System")]
public partial class SaveSystem : GameSystem
{
    [SerializeField]
    private GameEvent onNewGameStarted;

    [SerializeField]
    private StringEvent onSceneWarp;

    [SerializeField]
    private FloatEvent onWarpStart;

    [SerializeField]
    private FloatEvent onWarpEnd;

    [SerializeField]
    private IntReference saveSlot;

    [SerializeField]
    private StringReference playerName;

    [SerializeField]
    private StringReference farmName;

    [System.NonSerialized]
    private bool isNewGame;

    private SaveData cachedSaveData;

    public override void OnLoadSystem()
    {
        string saveJson;
        SaveMaster.GetMetaData("savedata", out saveJson);

        if (String.IsNullOrEmpty(saveJson))
        {
            isNewGame = true;
            CreateNewSaveInfo();
        }
        else
        {
            cachedSaveData = JsonUtility.FromJson<SaveData>(saveJson);
        }

        onSceneWarp?.AddListener(OnSceneWarp);
    }

    private void OnSceneWarp(string scene)
    {
        cachedSaveData.lastScene = scene;
    }

    private void CreateNewSaveInfo()
    {
        cachedSaveData = new SaveData();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene getScene = SceneManager.GetSceneAt(i);

            if (getScene.name != "Core")
            {
                cachedSaveData.lastScene = getScene.name;
                cachedSaveData.playerName = playerName.Value;
                cachedSaveData.farmName = farmName.Value;
            }
        }
    }

    private void Start()
    {
        if (isNewGame)
        {
            onNewGameStarted.Invoke();
            SaveMaster.WriteActiveSaveToDisk();
            SaveMaster.SetMetaData("savedata", JsonUtility.ToJson(cachedSaveData));
        }
    }

    private void OnDestroy()
    {
        cachedSaveData.timePlayed = SaveMaster.GetSaveTimePlayed().ToString();
        SaveMaster.SetMetaData("savedata", JsonUtility.ToJson(cachedSaveData));
    }

}
