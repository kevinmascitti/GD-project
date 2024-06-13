using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //[SerializeField] private GameObject transitionCanvas;
    public void GoToLevelSelector()
    {
        //transitionCanvas.GetComponent<LevelLoader>().LoadNextLevel(SceneManager.GetActiveScene().buildIndex - 1);
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
