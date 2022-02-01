using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Actions/Open New Scene")]
public class ActionOpenScene : ScriptableObject
{
    [SerializeField]
    private string sceneName;

    public void LoadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        /*
        for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
        {
            Scene getScene = SceneManager.GetSceneAt(i);

            if (getScene != SceneManager.GetActiveScene())
            {
                SceneManager.UnloadSceneAsync(getScene.buildIndex);
            }
        }
        */

        SceneManager.LoadScene(sceneName);

        Resources.UnloadUnusedAssets();
    }
}
