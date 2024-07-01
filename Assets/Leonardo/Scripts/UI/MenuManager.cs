using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _inGameMenu;
    [SerializeField] private GameObject _GameOverMenu;
    [SerializeField] private GameObject[] _canvases;

    public static EventHandler OnGameOver;
    public bool _isMenuActive = false;

    private void Start() {
        PlayerCharacter.OnGameOver += ActivateGameOverMenu;
    }

    void Update()
    {
        // // DEATH LOGIC
        if (Input.GetKeyDown(KeyCode.End))
        {
            OnGameOver?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isMenuActive)
            {
                DeactivateInGameMenu();
            }
            else
            {
                ActivateInGameMenu();
            }
        }
    }

    private void ActivateGameOverMenu(object sender, EventArgs e)
    {
        _GameOverMenu.SetActive(true);
    }

    public void ActivateInGameMenu()
    {       
        _inGameMenu.SetActive(true);
        Time.timeScale = 0f;
        _isMenuActive = true;
    }

    public void DeactivateInGameMenu()
    {
        Time.timeScale = 1f;
        foreach (GameObject canvas in _canvases)
        {
            canvas.SetActive(false);
        }
        _inGameMenu.SetActive(false);
        _isMenuActive = false;
    }

}
