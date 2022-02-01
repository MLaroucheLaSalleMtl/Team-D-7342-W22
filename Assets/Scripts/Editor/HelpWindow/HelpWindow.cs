using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PostprocessCallback : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        HelpWindow.DisplayHelpWindow();
    }
}

public class HelpWindow : EditorWindow
{
    [UnityEditor.Callbacks.DidReloadScripts]
    public static void DisplayHelpWindow()
    {
        if (dontShowAgain == false
            && Directory.Exists(Path.Combine(Application.dataPath, "Upload")))
        {
            Display(true); 
        }
    }

    [MenuItem("Tools/RPG Farming Kit Help Menu")]
    private static void OpenWindow()
    {
        Display(false);
    }

    public static HelpWindow Instance { private set; get; }

    private static bool projectInUploadFolder = false;
    private static bool dontShowAgain = false;

    public static void Display(bool openedAutomatically)
    {
        if (Instance == null)
        {
            dontShowAgain = EditorPrefs.GetBool("RPGFarmingKit_HideHelpWindow");

            if (openedAutomatically && dontShowAgain)
                return;

            string path = Path.Combine(Application.dataPath, "Upload");
            projectInUploadFolder = Directory.Exists(path);

            var window = (HelpWindow)EditorWindow.GetWindow(typeof(HelpWindow));
            window.titleContent = new GUIContent("Help Menu");
            window.minSize = new Vector2(600, (projectInUploadFolder) ? 400 : 350);
            window.Show(!dontShowAgain); 

            Instance = window;
        }
    }

    private void OnEnable()
    {
        if (EditorPrefs.GetInt("FirstWindowLaunch", 0) == 0)
        {
            AddScenesToBuildList();
            EditorPrefs.SetInt("FirstWindowLaunch", 1);
        }
    }

    private void OnGUI()
    {
        if (projectInUploadFolder)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("We have detected that the project is located in the Upload folder.\n" +
                "This project uses a custom solution for loading in game assets for saving.\n" +
                "Therefore it is required to move content to the root folder.\n\n" +
                "Caution: if you have worked within the project already, \n" +
                "please make a backup before doing this.", EditorStyles.boldLabel);

            if (GUILayout.Button("MOVE UPLOAD CONTENT TO ROOT FOLDER", GUILayout.Height(35)))
            {
                MoveUploadContentToAssetRoot();
            } 

            bool setDontShowAgain = GUILayout.Toggle(dontShowAgain, "I understand. Dont show this popup again and let me use the tools below.");

            if (dontShowAgain != setDontShowAgain)
            {
                EditorPrefs.SetBool("RPGFarmingKit_HideHelpWindow", setDontShowAgain);
                dontShowAgain = setDontShowAgain;
            } 

            GUILayout.EndVertical();
        }

        if (!dontShowAgain && projectInUploadFolder)
        {
            GUI.enabled = false;
        }

        GUILayout.Space(5);

        GUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.Label("Helper functions", EditorStyles.boldLabel);


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Open Start Menu", GUILayout.Height(35)))
        {
            OpenStartMenu();
        }

        if (GUILayout.Button("Open Farm Scene + Core Scene", GUILayout.Height(35)))
        {
            OpenMainAndCoreScene();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Open Farm Framework Tools", GUILayout.Height(35)))
        {
            OpenKitTools();
        }

        if (GUILayout.Button("Open Project Documentation", GUILayout.Height(35)))
        {
            OpenProjectDocumentation();
        }

        if (GUILayout.Button("Rate the product (Asset Store Page)", GUILayout.Height(35)))
        {
            OpenAssetStorePage();
        }

        if (GUILayout.Button("Add all scenes to Build Settings", GUILayout.Height(35)))
        {
            AddScenesToBuildList();
        }

        GUI.enabled = true;

        GUILayout.EndVertical();

        GUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.Label("Do you have questions? Send an email to info@low-scope.com", EditorStyles.helpBox);

        GUILayout.EndVertical();

        GUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.Label("You can find this menu again by going to 'Tools/RPG Farming Kit Menu'", EditorStyles.boldLabel);

        GUILayout.EndVertical();
    }

    private void OpenAssetStorePage()
    {
        Application.OpenURL("https://assetstore.unity.com/packages/templates/packs/rpg-farming-kit-121080");
    }

    private void OpenProjectDocumentation()
    {
        Application.OpenURL("https://low-scope.com/projects/farmingkit/docs/#/");
    }

    private void OpenKitTools()
    {
        Selection.activeObject = FarmFrameworkTools.ObtainTools();
    }

    private void OpenStartMenu()
    {
        EditorSceneManager.SaveOpenScenes();

        var getScenePath = (projectInUploadFolder) ? "Assets/Upload/Scenes" : "Assets/Scenes";

        string levelStartMenu = "";

        foreach (var item in AssetDatabase.FindAssets("t:Scene", new string[1] { getScenePath }))
        {
            var path = AssetDatabase.GUIDToAssetPath(item);

            if (Path.GetFileNameWithoutExtension(path) == "StartMenu")
                levelStartMenu = path;
        }


        if (string.IsNullOrEmpty(levelStartMenu))
        {
            Debug.LogError("Could not find scene: StartMenu");
            return;
        }

        EditorSceneManager.OpenScene(levelStartMenu);
    }

    private void OpenMainAndCoreScene()
    {
        EditorSceneManager.SaveOpenScenes();

        var getScenePath = (projectInUploadFolder) ? "Assets/Upload/Scenes" : "Assets/Scenes";

        string levelFarmPath = "";
        string levelCorePath = "";

        foreach (var item in AssetDatabase.FindAssets("t:Scene", new string[1] { getScenePath }))
        {
            var path = AssetDatabase.GUIDToAssetPath(item);

            if (Path.GetFileNameWithoutExtension(path) == "Level_Farm")
                levelFarmPath = path;
            if (Path.GetFileNameWithoutExtension(path) == "Core")
                levelCorePath = path;
        }


        if (string.IsNullOrEmpty(levelCorePath) || string.IsNullOrEmpty(levelFarmPath))
        {
            Debug.LogError("Could not find scene: Core or Level_Farm");
            return;
        }

        EditorSceneManager.OpenScene(levelFarmPath);
        EditorSceneManager.OpenScene(levelCorePath, OpenSceneMode.Additive);
    }

    private void MoveUploadContentToAssetRoot()
    {
        if (projectInUploadFolder)
        {
            AssetDatabase.StartAssetEditing();

            // Copy any leftover things that are not assets
            string uploadFolderPath = Path.Combine(Application.dataPath, "Upload");

            uploadFolderPath = System.IO.Path.GetFullPath(uploadFolderPath);

#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            uploadFolderPath = uploadFolderPath.Replace("\\", "/");
#endif

            string[] filePaths = Directory.GetFiles(uploadFolderPath, "*", SearchOption.AllDirectories);
            int fileCount = filePaths.Length;

            for (int i = 0; i < fileCount; i++)
            {
#if UNITY_EDITOR_WIN
                string newPath = filePaths[i].Replace(@"\Upload", "");
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                string newPath = filePaths[i].Replace(@"/Upload", "");
#endif

                if (!File.Exists(newPath))
                {
                    var dirName = Path.GetDirectoryName(newPath);
                    if (!Directory.Exists(dirName))
                        Directory.CreateDirectory(dirName);

                    File.Move(filePaths[i], newPath);
                }
            }

            AssetDatabase.StopAssetEditing();

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            AssetDatabase.DeleteAsset("Assets/Upload");

            projectInUploadFolder = false;

            AddScenesToBuildList();

            foreach (SceneView item in SceneView.sceneViews)
            {
                item.in2DMode = true;
            }

            OpenMainAndCoreScene();

            AssetDatabase.SaveAssets();
        }
    }

    private static void AddScenesToBuildList()
    {
        List<EditorBuildSettingsScene> addableScenes = new List<EditorBuildSettingsScene>();

        var getScenePath = (projectInUploadFolder) ? "Assets/Upload/Scenes" : "Assets/Scenes";

        foreach (var item in AssetDatabase.FindAssets("t:Scene", new string[1] { getScenePath }))
        {
            addableScenes.Add(new EditorBuildSettingsScene(new GUID(item), true));
        }

        EditorBuildSettings.scenes = addableScenes.ToArray();
    }
}
