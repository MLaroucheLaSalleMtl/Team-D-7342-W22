using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SceneLoad : MonoBehaviour
{
    public Slider progressbar;
    public TextMesh Loadtext;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {

            yield return null;
            AsyncOperation operation = SceneManager.LoadSceneAsync("Core");
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                yield return null;
                if (progressbar.value < 1f)
                {
                    progressbar.value = Mathf.MoveTowards(progressbar.value, 1f, Time.deltaTime);
                }
                else
                { 
                    Loadtext.text = "Press SpaceBar";
                }
                if (Input.GetKeyDown(KeyCode.Space) && progressbar.value >= 1f)
                {
                    operation.allowSceneActivation = true;


                }
            }

        
    }

}
