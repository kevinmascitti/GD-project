using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCharacter : Character
{
    public float MAX_HP = 100;
    public float def_HP = 100;
    public float MAX_STAMINA = 100;
    public float def_STAMINA = 5;
    public int MAX_LIFE = 99;
    public int def_life = 5;
    
    public Slider sliderHP;
    public Slider sliderStamina;
    public TMP_Text UIExtraLife;
    private float currentStamina;
    private float staminaUp = 5;
    
    private int currentExtraLife;
    
    [SerializeField] private Camera camera;

    [NonSerialized] public RoomManager currentLevel;
    [NonSerialized] public Room currentRoom;

    public static EventHandler OnDeath;
    public static EventHandler OnGameOver;

    public static EventHandler<RoomArgs> OnStartRoom;
    public static EventHandler<RoomArgs> OnEndRoom;
    public static EventHandler<RoomArgs> OnNextRoom;
    public static EventHandler<RoomManagerArgs> OnStartLevel;
    public static EventHandler<RoomManagerArgs> OnEndLevel;

    // Start is called before the first frame update
    public void Awake()
    {
        isPlayer = true;
        sliderHP = GameObject.Find("HPBar").GetComponent<Slider>();
        sliderStamina = GameObject.Find("StaminaBar").GetComponent<Slider>();
        UIExtraLife = GameObject.Find("ExtraLifeUI").GetComponent<TMP_Text>();

        if (sliderHP && sliderStamina)
        {
            sliderHP.maxValue = MAX_HP;
            sliderStamina.maxValue = MAX_STAMINA;
        }
    
        UpdateHP(def_HP);
        UpdateStamina(def_STAMINA);
        UpdateExtraLife(def_life);

        LevelManager.OnInitializedLevels += SetCurrentRoom;
        LerpAnimation.OnEndAnimation += EndPlayerAnimation;
    }

    public void Start()
    {
        OnStartRoom?.Invoke(this, new RoomArgs(currentRoom));
        OnStartLevel?.Invoke(this, new RoomManagerArgs(currentLevel));
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Stamina++ " + staminaUp);
            if (currentStamina + staminaUp <= MAX_STAMINA)
            {
                currentStamina += staminaUp;
            }
            else
            {
                currentStamina = MAX_STAMINA;
            }

            UpdateStaminaUI(currentStamina);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            if (currentRoom.isLocked)
            {
                currentRoom.isLocked = false;
                Debug.Log("NEXT ROOM UNLOCKED");
            }
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void UpdateHP(float newHP)
    {
        currentHP = newHP;
        UpdateHPUI(currentHP);
    }
    
    public void UpdateStamina(float newStamina)
    {
        currentStamina = newStamina;
        UpdateStaminaUI(currentStamina);
    }
    
    public void UpdateExtraLife(int newExtraLife)
    {
        currentExtraLife = newExtraLife;
        UpdateExtraLifeUI(currentExtraLife);
    }

    public void Die()
    {
        OnEndRoom?.Invoke(this, new RoomArgs(currentRoom));
        
        UpdateHP(MAX_HP);
        UpdateStamina(0);
        UpdateExtraLife(currentExtraLife-1);
        
        // animazione personaggio che muore
        
        if (currentExtraLife < 0)
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
        
        currentRoom = currentLevel.firstRoom;
        gameObject.transform.position = currentLevel.rooms[0].spawnPoint;
        camera.transform.position = currentLevel.rooms[0].cameraPosition;
        Debug.Log("DIED");
    }

    public void GameOver()
    {
        OnEndLevel?.Invoke(this, new RoomManagerArgs(currentLevel));
        OnGameOver?.Invoke(this, EventArgs.Empty);
        
        // UI visibile
        Debug.Log("GAMEOVER");
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !currentRoom.isLocked)
        {
            OnEndRoom?.Invoke(this, new RoomArgs(currentRoom));
            OnNextRoom?.Invoke(this, new RoomArgs(currentRoom));
            
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;

            if (currentRoom.nextRoom.level == currentLevel) // CAMBIO STANZA
            {
                gameObject.GetComponent<LerpAnimation>().StartAnimation(transform.position, currentRoom.nextRoom.spawnPoint);
                camera.GetComponent<LerpAnimation>().StartAnimation(camera.transform.position, currentRoom.nextRoom.cameraPosition);
                currentRoom = currentRoom.nextRoom;
            }
            else // CAMBIO LIVELLO
            {
                // VFX o animazione cambio livello
                gameObject.transform.position = currentRoom.nextRoom.spawnPoint;
                camera.transform.position = currentRoom.nextRoom.cameraPosition;
                currentRoom = currentRoom.nextRoom;
                NewLevel();
            }
            
            if (currentRoom.level != currentLevel)
            {
                currentLevel = currentRoom.level;
                OnEndLevel?.Invoke(this, new RoomManagerArgs(currentLevel));
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("DeathGround"))
        {
            Die();
        }
    }

    public void UpdateHPUI(float HP)
    {
        if(sliderHP)
            sliderHP.value = HP;
    }

    public void UpdateStaminaUI(float stamina)
    {
        if (sliderStamina)
            sliderStamina.value = stamina;
    }
    
    public void UpdateExtraLifeUI(int extraLife)
    {
        if(UIExtraLife && extraLife >= 0)
            UIExtraLife.text = "x" + extraLife.ToString("00");
    }

    public void SetCurrentRoom(object sender, LevelManagerArgs args)
    {
        currentLevel = args.firstLevel;
        currentRoom = currentLevel.firstRoom.GetComponent<Room>();
    }

    private void EndPlayerAnimation(object sender, AnimationArgs args)
    {
        if (args.Obj == gameObject)
        {
            NewRoom();
        }
    }

    private void NewLevel()
    {
        OnStartLevel?.Invoke(this, new RoomManagerArgs(currentLevel));
        Debug.Log("NEW LEVEL");
        
        NewRoom();
    }

    private void NewRoom()
    {
        OnStartRoom?.Invoke(this, new RoomArgs(currentRoom));
        Debug.Log("NEW ROOM");
            
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }

}

public class RoomArgs : EventArgs
{
    public RoomArgs(Room a)
    {
        room = a;
    }

    public Room room;
}

public class RoomManagerArgs : EventArgs
{
    public RoomManagerArgs(RoomManager a)
    {
        level = a;
    }

    public RoomManager level;
}

public class LevelManagerArgs : EventArgs
{
    public LevelManagerArgs(List<RoomManager> a, RoomManager b)
    {
        levels = a;
        firstLevel = b;
    }

    public List<RoomManager> levels;
    public RoomManager firstLevel;
}