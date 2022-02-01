using Lowscope.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Actions/Load Game")]
public class ActionLoadGame : ScriptableObject
{
    [SerializeField]
    private StringVariable playerScene;

    public void Execute(int slotNumber)
    {
        SaveMaster.SetSlot(slotNumber, true);

        string saveJson = "";
        SaveData saveData;

        SaveMaster.GetMetaData("savedata", out saveJson);
        if (!string.IsNullOrEmpty(saveJson))
        {
            saveData = JsonUtility.FromJson<SaveData>(saveJson);

            SceneManager.LoadScene(saveData.lastScene);
            SceneManager.LoadScene(playerScene.Value, LoadSceneMode.Additive);
        }
    }
}