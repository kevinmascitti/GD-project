using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCharacter : Character
{
    public float MAX_HP = 100;
    public float def_HP = 100;
    public float MAX_STAMINA = 100;
    public float def_increase_STAMINA = 5;
    public int MAX_LIFE = 4;
    public int def_life = 4;
    
    public Slider sliderHP;
    public Slider sliderStamina;
    public GameObject UIExtraLife;
    private float currentStamina;
    private float staminaUp = 5;
    
    private int currentExtraLife;
    private List<GameObject> heartList = new List<GameObject>();

    [SerializeField] private Camera camera;

    [NonSerialized] public RoomManager currentLevel;
    [NonSerialized] public Room currentRoom;

    [NonSerialized] public Grabbable grabbableItem = null;
    [NonSerialized] public Grabbable grabbedItem = null;
    public GameObject grabbingHand;
    [NonSerialized] private List<Grabbable> nearGrabbables = new List<Grabbable>();
    
    // [SerializeField] private GameObject shoulder;
    // [SerializeField] private GameObject upperArm;
    // [SerializeField] private GameObject mediumArm;
    // [SerializeField] private GameObject hand;
    // [SerializeField] private GameObject knuckles;
    
    public static EventHandler OnDeath;
    public static EventHandler OnGameOver;
    public static EventHandler OnStaminaFull;

    public static EventHandler<RoomArgs> OnStartRoom;
    public static EventHandler<RoomArgs> OnEndRoom;
    public static EventHandler<RoomArgs> OnNextRoom;
    public static EventHandler<RoomManagerArgs> OnStartLevel;
    public static EventHandler<RoomManagerArgs> OnEndLevel;
    public static EventHandler<GrabbableArgs> OnGrabbed;
    public static EventHandler<GrabbableArgs> OnUsed;
    public static EventHandler<GrabbableArgs> OnComputedNearestGrabbable;

    // Start is called before the first frame update
    public void Awake()
    {
        isPlayer = true;
        sliderHP = GameObject.Find("HPBar").GetComponent<Slider>();
        sliderStamina = GameObject.Find("StaminaBar").GetComponent<Slider>();
        UIExtraLife = GameObject.Find("ExtraLife");

        if (sliderHP && sliderStamina)
        {
            sliderHP.maxValue = MAX_HP;
            sliderStamina.maxValue = MAX_STAMINA;
        }

        if (UIExtraLife)
        {
            for (int i = 0; i < MAX_LIFE; i++)
            {
                heartList.Add(Instantiate((GameObject) Resources.Load("Heart"), UIExtraLife.transform));
                heartList[i].SetActive(false);
            }
        }
    
        UpdateHP(def_HP);
        UpdateStamina(0);
        UpdateExtraLife(def_life);

        LevelManager.OnInitializedLevels += SetCurrentRoom;
        LerpAnimation.OnEndAnimation += EndPlayerAnimation;

        Grabbable.OnInsideRange += AddGrabbableInRange;
        Grabbable.OnOutsideRange += RemoveGrabbableInRange;
        Grabbable.OnGrab += StartMovingUpArm;
        Grabbable.OnThrow += StartMovingDownArm;
        Grabbable.OnUse += StartMovingDownArm;

        ComboCounter.OnCounterIncreased += IncreaseStamina;

        RemoteController.OnControllerAbility += EmptyStamina;

        Boss.OnBossDeath += UnlockRoom;
        Boss.OnBossDeath += UnlockButton;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        
        // AUMENTO STAMINA
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
        } // SBLOCCO LIVELLO SUCCESSIVO
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
        
        // LANCIO OGGETTO
        if (Input.GetKeyDown(KeyCode.G) && grabbableItem && grabbedItem == null && grabbableItem.GetState() == GrabbableState.GRABBABLE)
        {
            grabbedItem = grabbableItem;
            grabbedItem.Grab();
            OnGrabbed?.Invoke(this, new GrabbableArgs(grabbedItem));
            grabbableItem = null;
        }
        else if (Input.GetKeyDown(KeyCode.G) && grabbedItem && grabbedItem.GetState() == GrabbableState.GRABBED) 
        {
            if (grabbedItem.GetThrowState())
            {
                grabbedItem.Throw();
                grabbedItem = null;
            }
            else
            {
                grabbedItem.Use();
                grabbedItem = null;
            }
            OnUsed?.Invoke(this, new GrabbableArgs(grabbedItem));
        }
    }

    public void UpdateHP(float newHP)
    {
        currentHP = newHP;
        UpdateHPUI(currentHP);
    }
    
    public void UpdateStamina(float newStamina)
    {
        if (newStamina >= MAX_STAMINA)
        {
            currentStamina = MAX_STAMINA;
            OnStaminaFull?.Invoke(this, EventArgs.Empty);
        }
        else
            currentStamina = newStamina;
        UpdateStaminaUI(currentStamina);
    }
    
    private void IncreaseStamina(object sender, int args)
    {
        UpdateStamina(currentStamina+def_increase_STAMINA);
    }

    private void EmptyStamina(object sender, EventArgs args)
    {
        UpdateStamina(0);
    }
    
    public void UpdateExtraLife(int newExtraLife)
    {
        currentExtraLife = newExtraLife;
        UpdateExtraLifeUI(currentExtraLife);
    }

    public override void Die()
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

    private void UpdateHPUI(float HP)
    {
        if(sliderHP)
            sliderHP.value = HP;
    }

    private void UpdateStaminaUI(float stamina)
    {
        if (sliderStamina)
            sliderStamina.value = stamina;
    }
    
    private void UpdateExtraLifeUI(int extraLife)
    {
        if (UIExtraLife && extraLife >= 0)
        {
            for (int i = 0; i < MAX_LIFE; i++)
            {
                if(i < currentExtraLife)
                    heartList[i].SetActive(true);
                else
                    heartList[i].SetActive(false);
            }
        }
    }

    public void SetCurrentRoom(object sender, LevelManagerArgs args)
    {
        currentLevel = args.firstLevel;
        currentRoom = currentLevel.firstRoom.GetComponent<Room>();
        
        transform.position = currentRoom.spawnPoint;
        
        OnStartRoom?.Invoke(this, new RoomArgs(currentRoom));
        OnStartLevel?.Invoke(this, new RoomManagerArgs(currentLevel));
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
        OnStartLevel?.Invoke(this, new RoomManagerArgs(currentRoom.level));
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

    private void AddGrabbableInRange(object sender, GrabbableArgs args)
    {
        nearGrabbables.Add(args.grabbable);
        ComputeNearestGrabbable();
    }
    
    private void RemoveGrabbableInRange(object sender, GrabbableArgs args)
    {
        nearGrabbables.Remove(args.grabbable);
        ComputeNearestGrabbable();
    }
    
    private void ComputeNearestGrabbable()
    {
        if (nearGrabbables.Count == 0)
        {
            grabbableItem = null;
        }
        else
        {
            nearGrabbables.Sort((a, b) =>
            {
                if (Vector3.Distance(a.gameObject.transform.position, transform.position) > Vector3.Distance(b.gameObject.transform.position, transform.position))
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
            // PRENDO L'OGGETTO PIU VICINO
            grabbableItem = nearGrabbables[0];
        }

        OnComputedNearestGrabbable?.Invoke(this, new GrabbableArgs(grabbableItem));
    }

    private void StartMovingUpArm(object sender, GrabbableArgs args)
    {
        GetComponent<Animator>().SetTrigger("GrabObject");
        // GetComponent<Animator>().enabled = false;
        // StartCoroutine(MoveUpArm());
    }
    
    // private IEnumerator MoveUpArm()
    // {
    //     yield return new WaitForSeconds(0.2f);
    //     
    //     float angle = shoulder.transform.localRotation.eulerAngles.x;
    //     while((shoulder.transform.localRotation.eulerAngles.x % 360) < angle + 50)
    //     {
    //         shoulder.transform.Rotate(-Vector3.up, 500 * Time.deltaTime, Space.Self);
    //         upperArm.transform.RotateAround(shoulder.transform.Find("Center").transform.position, Vector3.forward, 500 * Time.deltaTime);
    //         mediumArm.transform.RotateAround(shoulder.transform.Find("Center").transform.position, Vector3.forward, 500 * Time.deltaTime);
    //         hand.transform.RotateAround(shoulder.transform.Find("Center").transform.position, Vector3.forward, 500 * Time.deltaTime);
    //         knuckles.transform.RotateAround(shoulder.transform.Find("Center").transform.position, Vector3.forward, 500 * Time.deltaTime);
    //         yield return null;
    //     }
    // }

    private void StartMovingDownArm(object sender, GrabbableArgs args)
    {
        GetComponent<Animator>().SetTrigger("UseOrThrowObject");
        // StartCoroutine(MoveDownArm());
    }
    
    // private IEnumerator MoveDownArm()
    // {
    //     float angle = shoulder.transform.localRotation.eulerAngles.x;
    //     while((shoulder.transform.localRotation.eulerAngles.x % 360) > angle - 50)
    //     {
    //         shoulder.transform.Rotate(Vector3.up, 500 * Time.deltaTime, Space.Self);
    //         upperArm.transform.RotateAround(shoulder.transform.Find("Center").transform.position, -Vector3.forward, 500 * Time.deltaTime);
    //         mediumArm.transform.RotateAround(shoulder.transform.Find("Center").transform.position, -Vector3.forward, 500 * Time.deltaTime);
    //         hand.transform.RotateAround(shoulder.transform.Find("Center").transform.position, -Vector3.forward, 500 * Time.deltaTime);
    //         knuckles.transform.RotateAround(shoulder.transform.Find("Center").transform.position, -Vector3.forward, 500 * Time.deltaTime);
    //         yield return null;
    //     }
    //
    //     GetComponent<Animator>().enabled = true;
    // }

    private void UnlockRoom(object sender, ControllerButton newButton)
    {
        if (currentRoom.isLocked)
        {
            currentRoom.isLocked = false;
            Debug.Log("NEXT ROOM UNLOCKED");
        }
    }
    
    private void UnlockButton(object sender, ControllerButton newButton)
    {
        // TO DO: unlock button
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

public class GrabbableArgs : EventArgs
{
    public GrabbableArgs(Grabbable a)
    {
        grabbable = a;
    }

    public Grabbable grabbable;
}