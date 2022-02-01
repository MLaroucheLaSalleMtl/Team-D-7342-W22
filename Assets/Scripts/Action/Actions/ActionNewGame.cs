using Lowscope.Saving;
using Lowscope.Saving.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Actions/New Game" )]
public class ActionNewGame : ScriptableObject
{
    [SerializeField]
    private StringVariable firstScene;

    [SerializeField]
    private StringVariable playerScene;

    public void Execute()
    {
        int getUnusedSlot = SaveFileUtility.GetAvailableSaveSlot();
        SaveMaster.SetSlot(getUnusedSlot, true);

        SceneManager.LoadScene(firstScene.Value);
        SceneManager.LoadScene(playerScene.Value, LoadSceneMode.Additive);
    }
}
