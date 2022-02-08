using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update

      public void StartButton(){
        SceneManager.LoadScene("Core");
    }

    public void ExitButton() {
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
