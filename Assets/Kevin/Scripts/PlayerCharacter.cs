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

    [NonSerialized] public LevelManager levels;
    [NonSerialized] public RoomManager currentLevel;
    [NonSerialized] public Room currentRoom;

    public static EventHandler OnDeath;
    public static EventHandler OnGameOver;

    public static EventHandler<ChangeRoomArgs> OnStartRoom;
    public static EventHandler<ChangeRoomArgs> OnEndRoom;
    public static EventHandler OnEndLevel;

    // Start is called before the first frame update
    void Awake()
    {
        if (sliderHP && sliderStamina)
        {
            sliderHP.maxValue = MAX_HP;
            sliderStamina.maxValue = MAX_STAMINA;
            extraLife = INITIAL_LIFE;
        }

        RoomManager.OnInitializedLevel += UseLevelManager;
        LerpAnimation.OnEndAnimation += EndedRoomAnimation;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        
        if (Input.GetKeyDown(KeyCode.E))
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
        else if (Input.GetKeyDown(KeyCode.L))
        {
            currentRoom.isLocked = false;
        }

        if (sliderHP && sliderStamina)
        {
            sliderHP.value = currentHP;
            sliderStamina.value = currentStamina;
        }
        
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        currentHP = MAX_HP;
        currentStamina = 0;
        extraLife--;
        
        // animazione personaggio che muore
        
        if (extraLife < 0)
        {
            GameOver();
        }
        else
        {
            // VFX nuvoletta di respawn e transizione con timer
            Respawn();
        }
    }

    public void Respawn()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
        gameObject.transform.position = currentLevel.rooms[0].spawnPoint;
        camera.transform.position = currentLevel.rooms[0].cameraPosition;
        Debug.Log("DIED");
    }

    public void GameOver()
    {
        // UI visibile
        OnGameOver?.Invoke(this, EventArgs.Empty);
        Debug.Log("GAMEOVER");
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !currentRoom.isLocked)
        {
            OnEndRoom?.Invoke(this, new ChangeRoomArgs(currentRoom));
            
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;

            if (currentRoom.nextRoom.level == currentLevel)
            {
                gameObject.GetComponent<LerpAnimation>().StartAnimation(transform.position, currentRoom.nextRoom.spawnPoint);
                camera.GetComponent<LerpAnimation>().StartAnimation(camera.transform.position, currentRoom.nextRoom.cameraPosition);
            }
            else
            {
                // VFX o animazione cambio livello
                gameObject.transform.position = currentRoom.nextRoom.spawnPoint;
                camera.transform.position = currentRoom.nextRoom.cameraPosition;
                NewRoom();
            }
            
            currentRoom = currentRoom.nextRoom;
            if (currentRoom.level != currentLevel)
            {
                currentLevel = currentRoom.level;
                OnEndLevel?.Invoke(this, EventArgs.Empty);
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("DeathGround"))
        {
            Die();
        }
    }
    
    public void UseLevelManager(object sender, EventArgs args)
    {
        levels = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        currentLevel = levels.levels[0];
        currentRoom = currentLevel.rooms[0].GetComponent<Room>();
    }

    private void EndedRoomAnimation(object sender, AnimationArgs args)
    {
        if (args.Obj == gameObject)
        {
            NewRoom();
        }
    }

    private void NewRoom()
    {
        OnStartRoom?.Invoke(this, new ChangeRoomArgs(currentRoom));
            
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }

}

public class ChangeRoomArgs : EventArgs
{
    public ChangeRoomArgs(Room a)
    {
        room = a;
    }

    public Room room;
}