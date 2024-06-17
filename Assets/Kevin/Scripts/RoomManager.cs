using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    public static EventHandler OnInitializedLevel;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        ID = Int32.Parse(gameObject.name.Substring("Level".Length));
        deathGround = transform.Find("DeathGround").gameObject;
        
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

        PlayerCharacter.OnStartRoom += ClosePrevDoors;
        PlayerCharacter.OnNextRoom += OpenNextDoors;
        
        OnInitializedLevel?.Invoke(this, EventArgs.Empty);
    }

    public void OpenNextDoors(object sender, RoomArgs args)
    {
        if (args != null)
        {
            args.room.exitWall.isTrigger = true;
            args.room.nextRoom.enterWall.isTrigger = true;
        }
    }

    public void ClosePrevDoors(object sender, RoomArgs args)
    {
        if (args != null && args.room.prevRoom)
        {
            args.room.prevRoom.exitWall.isTrigger = false;
            args.room.enterWall.isTrigger = false;
        }
    }
    
    
}

