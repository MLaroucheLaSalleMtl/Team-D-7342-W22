using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectToggler : MonoBehaviour
{
    [SerializeField]
    private bool startState;

    private bool utilizedStartState;

    public void Toggle (GameObject target)
    {
        if (!utilizedStartState)
        {
            utilizedStartState = true;
            target.SetActive(startState);
            return;
        }

        target.SetActive(!target.activeSelf);
    }
}
