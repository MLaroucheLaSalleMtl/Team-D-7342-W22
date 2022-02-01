using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
#endif

using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ReferenceableAssetPostProcessor
{
    private static HashSet<string> guidCache;

#if UNITY_EDITOR

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnLoad()
    {
        LoadItems(true);
    }

    [PostProcessSceneAttribute(-1)]
    public static void OnPostprocessScene()
    {
        if (BuildPipeline.isBuildingPlayer)
        {
            LoadItems(false);
        }
    }

    private static void LoadItems(bool testingInEditor)
    {
        int buildIndex = EditorSceneManager.GetActiveScene().buildIndex;

        bool canCreateAssetDatabase = (testingInEditor) ? true : (buildIndex == 0);

        // We only want to create this object at the first scene.
        if (canCreateAssetDatabase)
        {
            guidCache = new HashSet<string>();

            if (BuildPipeline.isBuildingPlayer)
            {
                var scene = EditorSceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(scene);
            }

            GameObject assetDataBase = new GameObject("Scriptable Asset Database");

            EditorUtility.SetDirty(assetDataBase);

            ScriptableAssetDatabase dataBase = assetDataBase.AddComponent<ScriptableAssetDatabase>();

            EditorUtility.SetDirty(dataBase);

            string[] lookPath = new string[] { "Assets/ScriptableObjects" };
            string[] itemGuidPaths = AssetDatabase.FindAssets("t:ScriptableObject", lookPath);

            for (int i = 0; i < itemGuidPaths.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(itemGuidPaths[i]);
                IReferenceableAsset getItemData = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path) as IReferenceableAsset;

                if (getItemData != null)
                {
                    string guid = getItemData.GetGuid().ToString();
                    ScriptableObject asset = getItemData as ScriptableObject;

                    bool isGuidDuplicate = guidCache.Contains(guid);

                    // Check if guid is valid or duplicate, if so then we assign a new one for the asset.
                    // And we ensure this gets saved within the editor.
                    if (string.IsNullOrEmpty(getItemData.GetGuid()) || isGuidDuplicate)
                    {
                        getItemData.GenerateNewGuid();
                        guid = getItemData.GetGuid().ToString();

                        EditorUtility.SetDirty((ScriptableObject)getItemData);

                        Debug.Log($"Found duplicate guid on {getItemData}");
                    }

                    dataBase.values.Add(asset);
                    dataBase.keys.Add(guid);

                    // Fast way to do a lookup in case there is a duplicate guid
                    guidCache.Add(guid);
                }
            }
        }
    }

#endif
}

#if UNITY_EDITOR

/// <summary>
/// Keeps record of all ScriptableAssets, to ensure that every asset is unique with it's own GUID.
/// </summary>
[InitializeOnLoad]
public class ScriptableAssetCache
{
    public static Dictionary<string, string> cache;

    private static bool debugMode = false;

    private static void Log(string text)
    {
        if (debugMode)
        {
            Log(text);
        }
    }

    public static void Reload()
    {
        cache = new Dictionary<string, string>();

        string[] lookPath = new string[] { "Assets/ScriptableObjects" };
        string[] itemGuidPaths = AssetDatabase.FindAssets("t:ScriptableObject", lookPath);

        for (int i = 0; i < itemGuidPaths.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(itemGuidPaths[i]);
            IReferenceableAsset getItemData = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path) as IReferenceableAsset;

            if (getItemData != null)
            {
                Add(getItemData, path);
            }
        }

        Log($"Total unique scriptable assets found: {cache.Count}");
    }

    public static void TryRemoveFromPath(string path)
    {
        if (cache == null)
        {
            Reload();
            return;
        }

        string key = "";

        foreach (KeyValuePair<string, string> item in cache)
        {
            if (item.Value == path)
            {
                key = item.Key;
                break;
            }
        }

        if (cache.Remove(key))
        {
            Log($"Removed key at: {path}");
        }
    }

    public static void Add(IReferenceableAsset asset, string path)
    {
        if (cache == null)
        {
            Reload();
            return;
        }

        if (!cache.ContainsKey(asset.GetGuid()))
        {
            cache.Add(asset.GetGuid(), path);
        }
        else
        {
            if (!cache.ContainsValue(path))
            {
                Log($"Tried to add duplicate guid: {(asset as ScriptableObject).name}. Will create a new unique guid.");
                asset.GenerateNewGuid();
                Add(asset, path);
            }
        }
    }
}

class MyAllPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // Importing
        foreach (string path in importedAssets)
        {
            if (path.EndsWith(".asset"))
            {
                ScriptableAsset asset = AssetDatabase.LoadAssetAtPath<ScriptableAsset>(path);

                if (asset != null)
                {
                    ScriptableAssetCache.Add(asset, path);
                }
            }
        }

        // Removing
        foreach (var path in deletedAssets)
        {
            if (path.EndsWith(".asset"))
            {
                ScriptableAssetCache.TryRemoveFromPath(path);
            }
        }
    }
}

#endif