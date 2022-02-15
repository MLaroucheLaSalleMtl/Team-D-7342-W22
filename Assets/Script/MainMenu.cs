using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider slider; 
    
    // Start is called before the first frame update

      public void StartButton(int sceneIndex)
      {
        StartCoroutine (LoadAsynchronously(sceneIndex));
      }
    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        float timer = 0.0f;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingScreen.SetActive(true);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            
            yield return null;
            timer += Time.deltaTime;
            if(operation.progress < 0.9f)
            {
                slider.value = Mathf.Lerp(slider.value,operation.progress,timer);
                if(slider.value >= operation.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                slider.value = Mathf.Lerp(slider.value, 1f, timer);
                if(slider.value == 1.0f)
                {
                    operation.allowSceneActivation = true;
                    
                    yield break;
                }
            }
            
        }

    }

    public void ExitButton() 
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
