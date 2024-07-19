using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class RoomManager : MonoBehaviour
{
    [NonSerialized] public GameObject player;
    [NonSerialized] public int ID;
    [NonSerialized] public GameObject deathGround;
    [NonSerialized] public List<Room> rooms = new List<Room>();
    [NonSerialized] public RoomManager nextLevel;
    [NonSerialized] public Room firstRoom;
    [NonSerialized] public Room lastRoom;
    [NonSerialized] public bool isCompleted = false;
    [NonSerialized] public Transform inizio;
    [NonSerialized] public Transform fine;
    [SerializeField] private List<Sprite> layers;
    private GameObject background;

    public static EventHandler OnInitializedLevel;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        ID = Int32.Parse(gameObject.name.Substring("Level".Length));
        deathGround = transform.Find("DeathGround").gameObject;
        background = GameObject.Find("BackgroundCanvas");
        inizio = transform.Find("Inizio");
        fine = transform.Find("Fine");
        if (inizio == null || fine==null )
        {
            Debug.Log("non ho trovato inizio o fine");
        }
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<Room>())
                rooms.Add(transform.GetChild(i).GetComponent<Room>());
        }
        
        for(int i = 0; i < rooms.Count; i++)
        {
            rooms[i].plane = rooms[i].transform.Find("Plane").gameObject;
            rooms[i].enterWall = rooms[i].transform.Find("EnterWall").GetComponent<BoxCollider>();
            rooms[i].enterWall.gameObject.layer = LayerMask.NameToLayer("Default");
            rooms[i].exitWall = rooms[i].transform.Find("ExitWall").GetComponent<BoxCollider>();
            rooms[i].spawnPoint = rooms[i].transform.Find("SpawnPoint").transform.position;
            rooms[i].cameraPosition = rooms[i].transform.Find("Camera").transform.position;
            rooms[i].level = this;
            
            if(i != 0)
                rooms[i].prevRoom = rooms[i - 1].GetComponent<Room>();
            if (i < rooms.Count - 1)
                rooms[i].nextRoom = rooms[i + 1].GetComponent<Room>();
        }

        firstRoom = rooms[0];
        lastRoom = rooms[rooms.Count-1];

        PlayerCharacter.OnStartRoom += ClosePrevWalls;
        LevelManager.OnStartRoom += ClosePrevWalls;
        PlayerCharacter.OnNextRoom += OpenNextWalls;
        PlayerCharacter.OnStartLevel += ChangeBackground;
        
        OnInitializedLevel?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        PlayerCharacter.OnStartRoom -= ClosePrevWalls;
        LevelManager.OnStartRoom -= ClosePrevWalls;
        PlayerCharacter.OnNextRoom -= OpenNextWalls;
        PlayerCharacter.OnStartLevel -= ChangeBackground;
    }

    public void OpenNextWalls(object sender, RoomArgs args)
    {
        if (args != null)
        {
            args.room.exitWall.isTrigger = true;
            args.room.nextRoom.enterWall.isTrigger = true;
        }
    }

    public void ClosePrevWalls(object sender, RoomArgs args)
    {
        if (args != null && args.room.prevRoom)
        {
            args.room.prevRoom.exitWall.isTrigger = false;
            args.room.enterWall.isTrigger = false;
        }
    }

    private void ChangeBackground(object sender, RoomManagerArgs args)
    {
        if (args.level == this)
        {
            int layerNumber = 0;
            foreach (Transform layer in background.transform)
            {
                if (layers.Count > layerNumber)
                {
                    foreach (Transform bgX in layer)
                    {
                        if (layers[layerNumber])
                            bgX.gameObject.GetComponent<Image>().sprite = layers[layerNumber];
                    }
                    layerNumber++;
                }
            }
        }
    }
}

