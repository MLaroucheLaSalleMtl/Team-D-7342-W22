using UnityEngine;
using System.Collections;

[System.Serializable]
public struct ItemEnergy
{
    public float min;
    public float max;
    public float current;

    public bool IsDefault()
    {
        return min == 0 && max == 0 && current == 0;
    }
}