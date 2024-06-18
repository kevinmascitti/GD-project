using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public bool isLocked = false;
    
    [NonSerialized] public int ID;
    [NonSerialized] public GameObject plane;
    [NonSerialized] public Vector3 spawnPoint;
    [NonSerialized] public Vector3 cameraPosition;
    [NonSerialized] public Room prevRoom;
    [NonSerialized] public Room nextRoom;
    [NonSerialized] public RoomManager level;
    [NonSerialized] public BoxCollider enterWall;
    [NonSerialized] public BoxCollider exitWall;

    private Spawner spawner;
    [SerializeField] private bool isBossRoom = false;

    public void Awake()
    {
        ID = Int32.Parse(gameObject.name.Substring("Room".Length));
        spawner = transform.Find("spawner").GetComponent<Spawner>();
        
        PlayerCharacter.OnStartRoom += EnableRoom;
        PlayerCharacter.OnEndRoom += DisableRoom;
    }

    private void EnableRoom(object sender, RoomArgs args)
    {
        if (args.room == this && spawner)
        {
            spawner.SetEnable(true);
        }
    }
    
    private void DisableRoom(object sender, RoomArgs args)
    {
        if (args.room == this && spawner)
        {
            spawner.SetEnable(false);
        }
    }

    public void ClearEnterLayer()
    {
        enterWall.gameObject.layer = 0;
    }

    public void ClearExitLayer()
    {
        exitWall.gameObject.layer = 0;
    }
}
