using UnityEngine;
using System.Collections;

[System.Serializable]
public class ScriptableVariable<T> : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private T value;

    private T runtimeValue;

    [System.NonSerialized]
    private bool initialized;

    public void SetRuntimeValue (T value)
    {
        Value = value;
    }

    public T Value
    {
        get
        {
            if (!initialized)
            {
                runtimeValue = value;
                initialized = true;
            }

            //Debug.Log($"{ this.name} loaded : {runtimeValue}");

            return runtimeValue;
        }

        set
        {
            initialized = true;

            hideFlags = HideFlags.DontUnloadUnusedAsset;

            //Debug.Log($"{ this.name} set : {value}");

            runtimeValue = value;
        }
    }

    public void OnAfterDeserialize()
    {

    }

    public void OnBeforeSerialize()
    {

    }
}