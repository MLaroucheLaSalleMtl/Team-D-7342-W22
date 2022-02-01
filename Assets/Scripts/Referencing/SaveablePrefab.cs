using Lowscope.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This scriptable object is used to obtain unique references to prefabs.
/// </summary>
[CreateAssetMenu(fileName = "Saveable Prefab", menuName = "Referencing/Saveable prefab")]
public class SaveablePrefab : ScriptableAsset
{
    [SerializeField]
    private GameObject prefab;

    public T Retrieve<T>(string identification = "", Scene scene = default) where T : UnityEngine.Object
    {
        var spawnedPrefab = SaveMaster.SpawnSavedPrefab(InstanceSource.Custom, GetGuid(), scene: scene, customSource: "ScriptableAssetDatabase", clearWhenDisabled: true);

        if (typeof(T) == typeof(GameObject))
        {
            return spawnedPrefab as T;
        }

        return spawnedPrefab.GetComponent<T>();
    }

    public GameObject GetPrefab()
    {
        return prefab;
    }
}