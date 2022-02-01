using UnityEngine;
using UnityEditor;

/// <summary>
/// Responnsible for tracking the camera location of the 
/// editor scene window. Used to place the character.
/// </summary>
[InitializeOnLoad, System.Serializable]
public class SceneViewCameraRecorder
{
    public static Vector3 CameraLocation;
    public static bool inEditMode;

    static SceneViewCameraRecorder()
    {
        EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;
    }

    ~SceneViewCameraRecorder()
    {
        EditorApplication.playModeStateChanged -= HandleOnPlayModeChanged;
    }

    private static void HandleOnPlayModeChanged(PlayModeStateChange obj)
    {
        if (obj == PlayModeStateChange.ExitingEditMode)
        {
            SceneView.RepaintAll();
            Camera[] cameras = SceneView.GetAllSceneCameras();

            Camera validCamera = null;

            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i] != null && !cameras[i].CompareTag("MainCamera"))
                {
                    validCamera = cameras[i];
                    break;
                }
            }

            if (validCamera != null && validCamera.transform.position != Vector3.zero)
            {
                CameraLocation = validCamera.transform.position;
            }

            EditorPrefs.SetFloat("SceneViewXPosition", CameraLocation.x);
            EditorPrefs.SetFloat("SceneViewYPosition", CameraLocation.y);
        }

        if (obj == PlayModeStateChange.EnteredPlayMode)
        {
            inEditMode = false;
        }

        if (obj == PlayModeStateChange.EnteredEditMode)
        {
            inEditMode = true;
        }
    }
}