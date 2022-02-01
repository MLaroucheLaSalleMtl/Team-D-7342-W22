using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "HitFlashConfiguration", menuName = "Game/Combat/HitFlashConfiguration")]
public class HitFlashConfiguration : ScriptableObject
{
    [SerializeField]
    private float hitIntensityMultiplier = 1;
    public float HitIntensityMultiplier
    {
        get { return hitIntensityMultiplier; }
    }

    [SerializeField]
    private Shader hitShader;
    public UnityEngine.Shader HitShader
    {
        get { return hitShader; }
    }

    [SerializeField]
    private Shader diffuseShader;
    public UnityEngine.Shader DiffuseShader
    {
        get { return diffuseShader; }
    }

    [SerializeField]
    private float hitFlashTime = 6;
    public float HitFlashTime
    {
        get { return hitFlashTime; }
    }

    [SerializeField]
    private float hitFlashIntensity = 14;
    public float HitFlashIntensity
    {
        get { return hitFlashIntensity; }
    }
}
