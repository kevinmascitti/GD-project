using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Unity.VisualScripting;
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
    
    private int extraLife;
    
    [SerializeField] private Camera camera;

    [SerializeField] private RoomManager currentLevel;
    [SerializeField] private Room currentRoom;

    // Start is called before the first frame update
    void Start()
    {
        if (sliderHP && sliderStamina)
        {
            sliderHP.maxValue = MAX_HP;
            sliderStamina.maxValue = MAX_STAMINA;
            extraLife = INITIAL_LIFE;
        }
        LerpAnimation.OnEndAnimation += SetWallsTriggers;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Stamina++ " + staminaDefault);
            if (currentStamina + staminaDefault <= MAX_STAMINA)
            {
                currentStamina += staminaDefault;
            }
            else
            {
                currentStamina = MAX_STAMINA;
            }
        }

        if (sliderHP && sliderStamina)
        {
            sliderHP.value = currentHP;
            sliderStamina.value = currentStamina;
        }
        
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
        SceneManager.LoadScene(currentLevel.rooms[0].scene.name);
        gameObject.transform.position = currentRoom.spawnPoint.transform.position;
        Debug.Log("DIED");
    }

    public void GameOver()
    {
        // UI visibile
        Debug.Log("GAMEOVER");
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !currentRoom.isLocked)
        {
            // isTrigger del prossimo livello a true
            currentRoom.exitWall.isTrigger = true;
            currentRoom.nextRoom.enterWall.isTrigger = true;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;

            gameObject.GetComponent<LerpAnimation>().StartAnimation(transform.position, currentRoom.nextRoom.spawnPoint.transform.position);
            camera.GetComponent<LerpAnimation>().StartAnimation(camera.transform.position, currentRoom.nextRoom.cameraPosition.transform.position);

            currentRoom = currentRoom.nextRoom;
        }
    }

    private void SetWallsTriggers(object sender, AnimationArgs args)
    {
        if (args.Obj == gameObject)
        {
            currentRoom.nextRoom.enterWall.isTrigger = false;
            currentRoom.exitWall.isTrigger = false;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

}