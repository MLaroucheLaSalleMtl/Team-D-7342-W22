using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Lowscope.ScriptableObjectUpdater;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Actions/Play Sound")]
public class ActionPlaySound : ScriptableObject
{
    [SerializeField, HideInInspector]
    private static AudioSource fxAudioSource;

    [SerializeField, HideInInspector]
    private static AudioSource musicAudioSource;

    [SerializeField]
    private BoolEvent fxMutedEvent = null;

    [SerializeField]
    private FloatEvent fxVolumeEvent = null;

    [SerializeField]
    private AudioMixerGroup fxAudioMixer;

    [SerializeField]
    private FloatEvent musicVolumeEvent = null;

    [SerializeField]
    private BoolEvent musicMutedEvent = null;

    [SerializeField]
    private AudioMixerGroup musicAudioMixer;

    private bool musicEnabled = true;

    private bool fxEnabled = true;

    [System.Serializable]
    public class SaveConfig
    {
        public bool fxEnabled = true;
        public bool musicEnabled = true;

        public float fxVolume = 1;
        public float musicVolume = 0.5f;
    }

    private SaveConfig save;

    private float FxVolume
    {
        get
        {
            float volume;
            fxAudioMixer.audioMixer.GetFloat("FX Volume", out volume);

            return Math.Abs(volume / 80);
        }

        set
        {
            // Mixer sound gets represented in decibel. Which means -80 is fully silent, And 0 is normal.
            // The curve is based on a logarithmic scale. Which is why the Mathf.Log calculation is required.
            fxAudioMixer.audioMixer.SetFloat("FX Volume", Mathf.Log(Mathf.Lerp(0.001f, 1, value)) * 20);
        }
    }

    private float MusicVolume
    {
        get
        {
            float volume;
            fxAudioMixer.audioMixer.GetFloat("Music Volume", out volume);

            return Mathf.Abs(volume / 80);
        }

        set
        {
            // Mixer sound gets represented in decibel. Which means -80 is fully silent, And 0 is normal.
            // The curve is based on a logarithmic scale. Which is why the Mathf.Log calculation is required.
            fxAudioMixer.audioMixer.SetFloat("Music Volume", Mathf.Log(Mathf.Lerp(0.001f, 1, value)) * 20);
        }
    }

    [UpdateScriptableObject(eventType = EEventType.Awake)]
    public void Initialize()
    {
        if (musicAudioSource == null)
        {
            musicAudioSource = CreateAudioSource();
            musicAudioSource.outputAudioMixerGroup = musicAudioMixer;
        }

        if (fxAudioSource == null)
        {
            fxAudioSource = CreateAudioSource();
            fxAudioSource.outputAudioMixerGroup = fxAudioMixer;
        }

        var loadData = PlayerPrefs.GetString("config", "");
        if (!string.IsNullOrEmpty(loadData))
        {
            save = JsonUtility.FromJson<SaveConfig>(loadData);
        }
        else
        {
            save = new SaveConfig();
        }

        musicEnabled = save.musicEnabled;
        fxEnabled = save.fxEnabled;

        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

    private void WriteConfig()
    {
        // Writing a lot of data to playerprefs is not reccomended.
        // However for small configurations it is accepted.
        PlayerPrefs.SetString("config", JsonUtility.ToJson(save));
    }

    [UpdateScriptableObject(eventType = EEventType.Start)]
    public void OnGameStart()
    {
        SetFXVolume(save.fxVolume);
        SetMusicVolume(save.musicVolume);

        ResendEvents();
    }

    public void ResendEvents()
    {
        musicMutedEvent?.Invoke(!musicEnabled);
        musicVolumeEvent?.Invoke(save.musicVolume);
        fxVolumeEvent?.Invoke(save.fxVolume);
        fxMutedEvent?.Invoke(!fxEnabled);
    }

    public void SetFXVolume(float volume)
    {
        FxVolume = (fxEnabled) ? volume : 0;
        fxVolumeEvent?.Invoke(volume);

        save.fxVolume = volume;
        WriteConfig();
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = (musicEnabled) ? volume : 0;
        musicVolumeEvent?.Invoke(volume);

        save.musicVolume = volume;
        WriteConfig();
    }

    public void ToggleSound()
    {
        fxEnabled = !fxEnabled;

        FxVolume = (fxEnabled) ? save.fxVolume : 0;

        fxMutedEvent?.Invoke(!fxEnabled);

        save.fxEnabled = fxEnabled;

        WriteConfig();
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;

        MusicVolume = (musicEnabled) ? save.musicVolume : 0;

        musicMutedEvent?.Invoke(!musicEnabled);

        save.musicEnabled = musicEnabled;

        WriteConfig();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicAudioSource.clip != clip)
        {
            musicAudioSource.Stop();

            musicAudioSource.loop = true;
            musicAudioSource.clip = clip;

            if (musicAudioSource.enabled)
            {
                musicAudioSource.Play();
            }
        }
    }

    public void PlaySoundCollection(SoundCollection soundCollection)
    {
        if (soundCollection != null)
        {
            soundCollection.Play(fxAudioSource);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (fxAudioSource.enabled)
        {
            fxAudioSource.PlayOneShot(clip);
        }
    }

    private AudioSource CreateAudioSource()
    {
        AudioSource newSource = new GameObject().AddComponent<AudioSource>();
        GameObject.DontDestroyOnLoad(newSource.gameObject);

#if UNITY_EDITOR
        newSource.name = "Audio Source";
#endif
        return newSource;
    }
}
