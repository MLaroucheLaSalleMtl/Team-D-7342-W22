using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : SingletonMonoBehaviour<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;
    public SceneName startingSceneName;

    private IEnumerator Fade(float finalAlpha)
    {
        isFading = true;
        faderCanvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        isFading = false;
        faderCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        //Call before scene unload fade out event
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        //Start fading to black and wait for it to finish before continuing
        yield return StartCoroutine(Fade(1f));

        //Set player position
        Player.Instance.gameObject.transform.position = spawnPosition;

        //Call before scene unload event
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        //Unload the current active scene
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        //Start loading the given scene and wait for it to finish
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        //Call after scene load event
        EventHandler.CallAfterSceneLoadEvent();

        //Start fading back in and wait for it to finish before exiting the function
        yield return StartCoroutine(Fade(0f));

        //Call after scene load fade in event
        EventHandler.CallAfterSceneLoadFadeInEvent();
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    private IEnumerator Start()
    {
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));
        EventHandler.CallAfterSceneLoadEvent();
        StartCoroutine(Fade(0f));
    }

    //This will be called when player wants to switch scenes
    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        //If a fade isn't happening then start fading andswitching scenes
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }
}
