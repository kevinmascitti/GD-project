using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCharacter : Character
{
    public const float MAX_HP = 100;
    public const float MAX_STAMINA = 100;
    public const int INITIAL_LIFE = 5;
    
    public Slider sliderHP;
    public Slider sliderStamina;
    private float currentStamina = 0;
    private float staminaDefault = 5;
    
    private const int initial_life = 5;
    private int extraLife = initial_life;
    
    private Level currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        sliderHP.maxValue = MAX_HP;
        sliderStamina.maxValue = MAX_STAMINA;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Stamina++ " + staminaDefault);
            currentStamina += staminaDefault;
        }
        
        sliderHP.value = currentHP;
        if (currentHP <= 0)
        {
            currentHP = MAX_HP;
            currentStamina = 0;
            extraLife--;
            if (extraLife == 0)
            {
                GameOver();
            }
            else
            {
                RespawnPlayer();
            }
        }
    }

    public void RespawnPlayer()
    {
        SceneManager.LoadScene(currentLevel.scene.name);
        gameObject.transform.position = currentLevel.spawnPosition;
        Debug.Log("DIED");
    }

    public void GameOver()
    {
        // UI visibile
        Debug.Log("GAMEOVER");
    }

}