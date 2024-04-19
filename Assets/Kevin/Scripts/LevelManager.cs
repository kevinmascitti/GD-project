using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    private int initializedLevels = 0;
    [SerializeField] private GameObject player;
    [SerializeField] private Camera camera;
    

    [NonSerialized] public List<RoomManager> levels = new List<RoomManager>();
    [NonSerialized] public RoomManager firstLevel;

    public static EventHandler OnFirstLevelChosen;

    public void Start()
    {
        player = GameObject.Find("Player");
        
        for(int i = 0; i < transform.childCount; i++)
        {
            levels.Add(transform.GetChild(i).GetComponent<RoomManager>());
        }
        
        for(int i = 0; i < levels.Count-1; i++)
        {
            if (i == 0)
            {
                levels[0].isFirstLevel = true;
                firstLevel = levels[0];
            }
            levels[i].GetComponent<RoomManager>().nextLevel = levels[i + 1].GetComponent<RoomManager>();
        }
        
        RoomManager.OnInitializedLevel += LinkLevels;
    }

    public void LinkLevels(object sender, EventArgs args)
    {
        initializedLevels++;
        if (initializedLevels == levels.Count)
        {
            player.transform.position = firstLevel.firstRoom.spawnPoint;
            camera.transform.position = firstLevel.firstRoom.cameraPosition;
            for (int i = 0; i < initializedLevels - 1; i++)
            {
                levels[i].lastRoom.nextRoom = levels[i + 1].firstRoom;
                levels[i + 1].firstRoom.prevRoom = levels[i].lastRoom;
            }
            firstLevel.firstRoom.ClearEnterLayer();
            levels[levels.Count-1].rooms[levels[levels.Count-1].rooms.Count-1].ClearExitLayer();
        }
    }
}
