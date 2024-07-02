using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public bool isLocked = true;
    
    [NonSerialized] public int ID;
    [NonSerialized] public GameObject plane;
    [NonSerialized] public Vector3 spawnPoint;
    [NonSerialized] public Vector3 cameraPosition;
    [NonSerialized] public Room prevRoom;
    [NonSerialized] public Room nextRoom;
    [NonSerialized] public RoomManager level;
    [NonSerialized] public BoxCollider enterWall;
    [NonSerialized] public BoxCollider exitWall;
    [NonSerialized] public Spawner spawner;
    private int killedEnemies = 0;
    [SerializeField] private bool isBossRoom = false;
    
    public static EventHandler<RoomArgs> OnEndRoom;

    public void Awake()
    {
        ID = Int32.Parse(gameObject.name.Substring("Room".Length));
        spawner = transform.Find("spawner").GetComponent<Spawner>();
        spawner.enabled = false;
        
        PlayerCharacter.OnStartRoom += EnableRoom;
        LevelManager.OnStartRoom += EnableRoom;
        Enemy.OnEnemyDeath += KillAndCheckEnemyCount;
    }

    private void EnableRoom(object sender, RoomArgs args)
    {
        if (args.room == this && spawner)
        {
            spawner.SetEnable(true);
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
    
    private void KillAndCheckEnemyCount(object sender, Room room)
    {
        killedEnemies++;
        if (room == this && killedEnemies == spawner.spawnLimit)
        {
            // GameObject.FindGameObjectWithTag("Spawner").GetComponent<Spawner>().OpenExit();
            spawner.SetEnable(false);
            room.spawner.spawnCount = 0;
            isLocked = false;
            OnEndRoom?.Invoke(this,new RoomArgs(this));
            killedEnemies = 0;
        }
    }

}
