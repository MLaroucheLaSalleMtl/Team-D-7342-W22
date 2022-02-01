using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using System;
using System.Linq;
using System.Collections;
using Lowscope.ScriptableObjectUpdater;

[CreateAssetMenu(fileName = "Farm Framework Tools", menuName = "Tools/Farm Framework Tools")]
public class FarmFrameworkTools : ScriptableObject
{
    private static readonly string assetName = "Farm Framework Tools.asset";
    private static readonly string folderPath = "Editor Default Resources";

    [System.Serializable]
    public class AutoCharacterLoading
    {
        public string gameplaySceneFolderName = "Levels";
        public List<string> autoLoadScenes = new List<string>();
        public bool setPlayerToCamera;
    }

    public AutoCharacterLoading autoCharacterLoading = new AutoCharacterLoading();


    [MenuItem("Tools/RPG Farm Framework Tools")]
    static void Open()
    {
        Selection.activeObject = ObtainTools();
    }

    public static FarmFrameworkTools ObtainTools()
    {
        FarmFrameworkTools toolData = EditorGUIUtility.Load(assetName) as FarmFrameworkTools;

        if (toolData == null)
        {
            if (!AssetDatabase.IsValidFolder($"Assets/{folderPath}"))
            {
                AssetDatabase.CreateFolder("Assets", folderPath);
            }

            toolData = new FarmFrameworkTools();

            AssetDatabase.CreateAsset(toolData, $"Assets/{folderPath}/{assetName}");
        }

        return toolData;
    }


    [UpdateScriptableObject(eventType = EEventType.Start, editorOnly = true)]
    public void OnLoad()
    {
        LoadAdditionalScenes();
    }

    [UpdateScriptableObject(eventType = EEventType.Start, Delay = 0.25f, editorOnly = true)]
    public void SetPlayerLocation()
    {
        if (autoCharacterLoading.setPlayerToCamera)
        {
            GameObject getPlayer = GameObject.FindWithTag("Player");

            if (getPlayer != null)
            {
                Vector2 cameraLocation = new Vector2()
                {
                    x = EditorPrefs.GetFloat("SceneViewXPosition", 0),
                    y = EditorPrefs.GetFloat("SceneViewYPosition", 0)
                };

                getPlayer.transform.position = cameraLocation;
            }
        }
    }

    private void LoadAdditionalScenes()
    {
        List<string> includedScenesOnPlay = autoCharacterLoading.autoLoadScenes;

        int sceneCount = EditorSceneManager.sceneCount;
        List<string> sceneNames = new List<string>();

        for (int i = 0; i < sceneCount; i++)
        {
            if (EditorSceneManager.GetSceneAt(i).path.Contains(autoCharacterLoading.gameplaySceneFolderName))
            {
                sceneNames.Add(EditorSceneManager.GetSceneAt(i).name);
            }
            else
            {
                return;
            }
        }

        for (int i = 0; i < includedScenesOnPlay.Count; i++)
        {
            if (!sceneNames.Contains(includedScenesOnPlay[i]))
            {
                EditorSceneManager.LoadScene(includedScenesOnPlay[i], LoadSceneMode.Additive);
            }
        }
    }

    [System.Serializable]
    public class GivePlayerItem
    {
        public ItemData item;
        public int amount;
        public bool give;
    }

    [SerializeField]
    GivePlayerItem givePlayerItem;

    public void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        if (givePlayerItem.give)
        {
            if (givePlayerItem.item == null)
                return;

            GameObject getPlayer = GameObject.FindWithTag("Player");

            Debug.Log("Cannot find player");

            if (getPlayer != null)
            {
                Debug.Log("Added item");

                Inventory getInventory = getPlayer.GetComponent<Inventory>();
                getInventory.AddItem(givePlayerItem.item, givePlayerItem.amount);
            }

            givePlayerItem.give = false;
        }
    }
}

public class CallbackObject : MonoBehaviour
{
    public Action OnDestroyAction = delegate { };

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(0.25f);
        GameObject.Destroy(this.gameObject);
    }

    public void OnDestroy()
    {
        OnDestroyAction.Invoke();
    }
}