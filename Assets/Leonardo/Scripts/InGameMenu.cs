using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InGameMenu : MonoBehaviour
{
    public void GoToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    

}
