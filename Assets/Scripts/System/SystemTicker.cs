using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemTicker : MonoBehaviour
{
    private List<GameSystem> systems = new List<GameSystem>();

    private List<GameSystem> updateSystems = new List<GameSystem>();
    private List<GameSystem> fixedUpdateSystems = new List<GameSystem>();

    private void Awake()
    {
        GetComponentsInChildren<GameSystem>(systems);

        foreach (GameSystem system in systems)
        {
            system.Initialize(this);

            if (system.systemSettings.hasFixedUpdate)
            {
                fixedUpdateSystems.Add(system);
            }

            if (system.systemSettings.hasUpdate)
            {
                updateSystems.Add(system);
            }
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < fixedUpdateSystems.Count; i++)
        {
            fixedUpdateSystems[i].FixedTick();
        }
    }

    private void Update()
    {
        for (int i = 0; i < updateSystems.Count; i++)
        {
            updateSystems[i].Tick();
        }
    }
}