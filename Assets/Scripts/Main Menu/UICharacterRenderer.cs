using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class UICharacterRenderer : MonoBehaviour
{
    private RenderTexture renderTexture;

    [SerializeField]
    private UnityEventRenderTexture onTextureMade;

    [SerializeField]
    private new Camera camera;

    [System.Serializable]
    public class UnityEventRenderTexture : UnityEvent<RenderTexture> { }

    // Use this for initialization
    void Awake()
    {
        renderTexture = new RenderTexture(64, 64, 16);
        renderTexture.Create();
        renderTexture.filterMode = FilterMode.Point;

        if (camera != null)
        {
            camera.targetTexture = renderTexture;
            camera.enabled = true;

            onTextureMade?.Invoke(renderTexture);
        }
    }

    public RenderTexture GetTexture()
    {
        return renderTexture;
    }
}
