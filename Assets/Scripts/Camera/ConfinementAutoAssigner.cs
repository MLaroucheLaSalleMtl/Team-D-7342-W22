using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// This class will automatically add a camera confinement reference
/// Each time the scene changes.
/// </summary>
public class ConfinementAutoAssigner : MonoBehaviour
{
    [SerializeField]
    private ScriptableReference cameraConfinement;

    [SerializeField]
    private StringEvent onSceneChanged;

    private CinemachineConfiner confiner;

    private void Awake()
    {
        onSceneChanged.AddListener(OnSceneChanged);

        GetConfinement();
    }

    private void OnSceneChanged(string scene)
    {
        GetConfinement();
    }

    private void GetConfinement()
    {
        GameObject getConfinementObject = cameraConfinement?.Reference;

        if (getConfinementObject != null)
        {
            Collider2D GetBoundingCollider = getConfinementObject.GetComponent<Collider2D>();

            if (GetBoundingCollider == null)
            {
                return;
            }

            if (confiner == null)
            {
                confiner = gameObject.GetComponent<CinemachineConfiner>();

                if (confiner == null)
                {
                    confiner = gameObject.AddComponent<CinemachineConfiner>();
                }
            }

            confiner.m_BoundingShape2D = GetBoundingCollider;
        }
    }
}
