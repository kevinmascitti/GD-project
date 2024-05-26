using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuInput : MonoBehaviour
{
    [SerializeField] private GameObject _inGameMenu;
    [SerializeField] private GameObject[] _canvases;

    [SerializeField] private GameObject _GameOverMenu;

    private bool _isInGameMenuActive;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _inGameMenu.SetActive(!_inGameMenu.activeSelf);
            if (_isInGameMenuActive) {
                foreach (GameObject canvas in _canvases)
                {
                    canvas.SetActive(false);
                    SetInGameMenuActive(false);
                }
            }
            SetInGameMenuActive(true);
        }

        // DEATH LOGIC
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (GameObject canvas in _canvases)
            {
                canvas.SetActive(false);
            }
            SetInGameMenuActive(false);
            _GameOverMenu.SetActive(true);
        }
    }

    public void SetInGameMenuActive(bool active)
    {
        _isInGameMenuActive = active;
    }
}
