using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Audio/Sound Collection", fileName = "Sound Collection")]
public class SoundCollection : ScriptableObject
{
    [SerializeField]
    private AudioClip[] clips;

    [SerializeField]
    private Vector2 pitchVariance = new Vector2(0.95f, 1.05f);

    public void Play (AudioSource audioSource)
    {
        if (clips.Length > 0 && audioSource.enabled)
        {
            audioSource.pitch = Random.Range(pitchVariance.x, pitchVariance.y);
            audioSource.PlayOneShot(clips[Random.Range(0, clips.Length - 1)]);
        }
    }
}
