using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ScriptableTileBase : TileBase, IReferenceableAsset
{
    [SerializeField, HideInInspector]
    private string guid;

    public void GenerateNewGuid()
    {
        guid = System.Guid.NewGuid().ToString();

#if UNITY_EDITOR
        Debug.Log($"Guid has been set on asset {this.name}");
        EditorUtility.SetDirty(this);
#endif
    }

    public string GetGuid()
    {
        if (string.IsNullOrEmpty(guid))
        {
            GenerateNewGuid();
        }

        return guid;
    }
}