using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[CreateAssetMenu]
public class WarpLocation : ScriptableAsset
{
    [SerializeField]
    private string scene;

    public string Scene { get { return scene; } }

    [SerializeField]
    private Vector2 position;

    public Vector2 Position { get { return position; } }

#if UNITY_EDITOR

    [SerializeField]
    private string setterName;

    [SerializeField]
    private string scenePath;

    public bool IsTarget (SceneWarper warper)
    {
        return warper != null && ( warper.name == setterName && warper.gameObject.scene.name == scene);
    }

    public void Set(SceneWarper warper)
    {
        position = warper.SpawnLocation;
        scene = warper.gameObject.scene.name;
        scenePath = warper.gameObject.scene.path;
        warper.name = $"WarpLocation_{scene}_{position}";
        setterName = warper.name;

        UnityEditor.EditorUtility.SetDirty(this);
    }

    public void GoToLocation()
    {
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }

        if (EditorSceneManager.GetActiveScene().name != scene)
        {
            Scene openScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            if (!openScene.IsValid())
            {
                return;
            }
        }

        GameObject target = GameObject.Find(setterName);

        if (target != null)
        {
            Vector3 position = SceneView.lastActiveSceneView.pivot;
            position.z -= 10.0f;
            SceneView.lastActiveSceneView.pivot = target.transform.position;
            SceneView.lastActiveSceneView.Repaint();
            Selection.activeGameObject = target;
        }

    }

#endif

}

#if UNITY_EDITOR

[CustomEditor(typeof(WarpLocation))]
public class WarpLocationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        base.OnInspectorGUI();
        GUI.enabled = true;

        if (GUILayout.Button("Go to location"))
        {
            ((WarpLocation)target).GoToLocation();
        }
    }
}

[CustomEditor(typeof(ScriptableObject))]
public class SCInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open asset folder"))
        {
            EditorGUIUtility.PingObject(Selection.activeObject);
        }

        base.OnInspectorGUI();
    }
}

#endif