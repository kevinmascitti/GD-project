using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InGameMenu : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void RestartScene() { 
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
