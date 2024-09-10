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
    [SerializeField] private int killedEnemies = 0;
    [SerializeField] private bool isBossRoom = false;
    public GameObject arrow;
    public static EventHandler<RoomArgs> OnEndRoom;
    public static EventHandler<RoomManager> OnLevelCompleted;

    private static List<RoomManager> levelsCompleted;

    public void Awake()
    {
        ID = Int32.Parse(gameObject.name.Substring("Room".Length));
        spawner = gameObject.transform.Find("spawner").GetComponent<Spawner>();
        spawner.enabled = false;
        levelsCompleted = new List<RoomManager>();
        
        PlayerCharacter.OnStartRoom += EnableRoom;
        LevelManager.OnStartRoom += EnableRoom;
        Enemy.OnEnemyDeath += KillAndCheckEnemyCount;
    }

    private void EnableRoom(object sender, RoomArgs args)
    {
        if (args.room == this && spawner)
        {
            spawner.SetEnable(true);
            arrow.SetActive(false);
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
        if (room == this)
        {
            killedEnemies++;
            if (killedEnemies >= spawner.spawnLimit)
            {
                // GameObject.FindGameObjectWithTag("Spawner").GetComponent<Spawner>().OpenExit();
                if (nextRoom == null || nextRoom.level != level)
                {
                    levelsCompleted.Add(level);
                    if (levelsCompleted.Count >= 3)
                    {
                        Debug.Log("END OF THE GAME!");
                        if (EndOfShowTransition.onGameEnd != null)
                            EndOfShowTransition.onGameEnd?.Invoke(this, EventArgs.Empty);
                        else 
                            Debug.LogError("EndOfShowTransition is null");
                        StartCoroutine(EndOfGameTimer());
                        return;
                    } 
                    OnLevelCompleted?.Invoke(this, level);
                }
                spawner.SetEnable(false);
                room.spawner.spawnCount = 0;
                isLocked = false;
                arrow.SetActive(true);
                OnEndRoom?.Invoke(this, new RoomArgs(this));
                killedEnemies = 0;
            }
        }
    }

    IEnumerator EndOfGameTimer() 
    {
        ChannelTransition ct = new ChannelTransition();
        yield return new WaitForSeconds(1);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(3);
    }
}
