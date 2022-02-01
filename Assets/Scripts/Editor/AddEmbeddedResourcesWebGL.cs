using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using UnityEngine;

#if UNITY_WEBGL

/// <summary>
/// Ensures that useEmbeddedResources is toggled on build
/// This is required to prevent errors related to Text Mesh Pro
/// </summary>

public class AddEmbeddedResourcesWebGL : IPreprocessBuild
{
    private static bool hasInitialized = false;

    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        if (!hasInitialized)
        {
            PlayerSettings.WebGL.useEmbeddedResources = true;
            hasInitialized = true;
        }
    }
}

#endif