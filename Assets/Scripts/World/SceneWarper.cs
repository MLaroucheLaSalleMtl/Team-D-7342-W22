using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

# if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class SceneWarper : MonoBehaviour
{
    // Scriptable object is to have a unique identification of a location.

    [SerializeField]
    private WarpLocation location;

    [SerializeField]
    private WarpLocation target;

    [SerializeField]
    private WarpEvent warpRequest;

    [SerializeField]
    private GameObject spawnLocation;

    public Vector2 SpawnLocation { get { return spawnLocation.transform.position; } }

    public WarpLocation Location { get { return location; } set { location = value; } }

    public WarpLocation Target { get { return target; } set { target = value; } }

    [SerializeField, HideInInspector]
    private BoxCollider2D boxCollider;

    [SerializeField, HideInInspector]
    private Rigidbody2D rigidBody;

#if UNITY_EDITOR

    [MenuItem("GameObject/2D Object/Utility/Scene Warper")]
    static void CreateInteractionFieldObject()
    {
        GameObject newObject = new GameObject("Scene Warper", typeof(SceneWarper));

        Selection.activeGameObject = newObject;
    }

    public void OnValidate()
    {
        if (this.transform.position.z != 0)
        {
            Vector3 newPosition = this.transform.position;
            newPosition.z = 0;
            this.transform.position = newPosition;
        }

        if (spawnLocation == null)
        {
            spawnLocation = new GameObject();
            spawnLocation.name = "Spawn Location";
            spawnLocation.transform.SetParent(this.transform);
            spawnLocation.transform.position = this.transform.position;
        }
        else
        {
            if (spawnLocation.transform.position.z != 0)
            {
                Vector3 newPosition = spawnLocation.transform.position;
                newPosition.z = 0;
                spawnLocation.transform.position = newPosition;
            }
        }

        // Ensure box collider is set to trigger once it is first referenced.
        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector2(0.2f, 0.1f);
        }

        // Ensure rigidbody body type is set to startic once it is first referenced.
        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Static;
        }
    }

    [SerializeField, HideInInspector]
    private Vector3 lastSpawnLocationPosition;

    [SerializeField, HideInInspector]
    private Color gizmoColor = Color.white;

    public void OnDrawGizmos()
    {
        if (lastSpawnLocationPosition != spawnLocation.transform.position)
        {
            Bounds getBounds = GetComponent<BoxCollider2D>().bounds;
            getBounds.Expand(0.05f);

            if (getBounds.Contains(spawnLocation.transform.position))
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            gizmoColor = Gizmos.color;

            Gizmos.DrawWireSphere(spawnLocation.transform.position, 0.05f);

            lastSpawnLocationPosition = spawnLocation.transform.position;
        }
        else
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(lastSpawnLocationPosition, 0.05f);
        }
    }

#endif

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (target != null)
            {
                warpRequest.Invoke(target);
            }
        }
    }
}

#if UNITY_EDITOR


[CustomEditor(typeof(SceneWarper))]
public class SceneWarperInspector : Editor
{
    private SceneWarper warperReference;

    private string warperLocationName;

    public override void OnInspectorGUI()
    {
        if (warperReference == null)
        {
            warperReference = (target as SceneWarper);
        }

        base.OnInspectorGUI();

        if (warperReference.Target == null)
            GUI.enabled = false;

        if (GUILayout.Button("Go to target location"))
        {
            warperReference.Target.GoToLocation();
        }

        GUI.enabled = true;

        if (warperReference.Location == null)
        {
            GUILayout.BeginVertical(GUI.skin.box);

            if (string.IsNullOrEmpty(warperLocationName))
            {
                warperLocationName = $"{warperReference.gameObject.scene.name}_";
            }

            warperLocationName = EditorGUILayout.TextField("Location Name", warperLocationName);

            if (GUILayout.Button("Create Warp Location Asset"))
            {
                if (string.IsNullOrEmpty(warperLocationName))
                {
                    warperLocationName = "location";
                }

                WarpLocation warpLocation = new WarpLocation();
                string uniquePath = AssetDatabase.GenerateUniqueAssetPath($"Assets/ScriptableObjects/Locations/{warperLocationName}.asset");

                AssetDatabase.CreateAsset(warpLocation, uniquePath);
                warperReference.Location = warpLocation;
                warperReference.OnValidate();

                warperReference.Location.Set(warperReference);

                EditorUtility.SetDirty(warperReference.gameObject);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                AssetDatabase.SaveAssets();
            }

            GUILayout.EndVertical();
        }

        // Is the current warper reference not the target of the location?
        // Then display the option to set it.
        if (!warperReference.Location.IsTarget(warperReference))
        {
            if (GUILayout.Button("Set Location Asset Data To This Location"))
            {
                warperReference.Location.Set(warperReference);
            }
        }
    }
}

#endif