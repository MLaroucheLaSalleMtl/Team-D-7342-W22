using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class Poolable : MonoBehaviour
{
    private ScriptablePoolContainer poolContainer;

    public void SetContainer(ScriptablePoolContainer _container)
    {
        poolContainer = _container;
    }

    private void OnDisable()
    {
        poolContainer.Return(this);
    }
}