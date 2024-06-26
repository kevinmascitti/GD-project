using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static EventHandler OnNewGame;
    
    //[SerializeField] private GameObject transitionCanvas;
    public void GoToLevelSelector()
    {
        if (OnNewGame != null)
        {
            foreach (var d in OnNewGame.GetInvocationList())
            {
                OnNewGame -= (EventHandler)d;
            }
        }
        //transitionCanvas.GetComponent<LevelLoader>().LoadNextLevel(SceneManager.GetActiveScene().buildIndex - 1);
        StopAllCoroutines();
        SceneManager.LoadScene(1, LoadSceneMode.Single);
        OnNewGame?.Invoke(this, EventArgs.Empty);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
