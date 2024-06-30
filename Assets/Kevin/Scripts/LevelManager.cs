using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{

    private int initializedLevels = 0;
    [SerializeField] private GameObject player;
    [SerializeField] private Camera camera;
    
    [NonSerialized] public List<RoomManager> levels = new List<RoomManager>();
    [NonSerialized] public RoomManager firstLevel;

    public static EventHandler<LevelManagerArgs> OnInitializedLevels;
    public static EventHandler<RoomArgs> OnStartRoom;
    public static EventHandler<RoomArgs> OnEndRoom;
    public static EventHandler<RoomManager> OnShuffleLevel; 

    public void Awake()
    {
        player = GameObject.Find("Player");
        
        for(int i = 0; i < transform.childCount; i++)
        {
            levels.Add(transform.GetChild(i).GetComponent<RoomManager>());
        }
        
        // TO DO: MISCHIARE LIVELLI con una funzione?

        for(int i = 0; i < levels.Count-1; i++)
        {
            if (i == 0)
            {
                firstLevel = levels[0]; 
            }
            levels[i].GetComponent<RoomManager>().nextLevel = levels[i + 1].GetComponent<RoomManager>();
        }
        
        RoomManager.OnInitializedLevel += LinkLevels;
        PlayerCharacter.OnRequestLevel += ChooseNextLevel;
        MainMenu.OnNewGame += NewGame;
        PlayerCharacter.OnStartLevel += StartLevel;
        PlayerCharacter.OnEndLevel += EndLevel;
    }

    private void LinkLevels(object sender, EventArgs args)
    {
        initializedLevels++;
        if (initializedLevels == levels.Count)
        {
            OnInitializedLevels?.Invoke(this, new LevelManagerArgs(levels, firstLevel));
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

    public void NewGame(object sender, EventArgs args)
    {
        foreach (RoomManager level in levels)
        {
            foreach (Room room in level.rooms)
            {
                room.isLocked = true;
            }
            level.isCompleted = false;
        }

        int firstLevelRand = Random.Range(0, levels.Count);
        firstLevel = levels[firstLevelRand];
        
        OnInitializedLevels?.Invoke(this, new LevelManagerArgs(levels, firstLevel));
    }

    private void ChooseNextLevel(object sender, RoomManager args)
    {
        bool isCurrentLevelChosen = true;
        RoomManager currentLevel = args;
        RoomManager levelToChoose;
        foreach (RoomManager level in levels)
        {
            if (!level.isCompleted && level != currentLevel)
            {
                isCurrentLevelChosen = false;
                break;
            }
        }
        
        if(isCurrentLevelChosen)
        {
            levelToChoose = currentLevel;
        }
        else
        {
            int rand = Random.Range(0, levels.Count);
            levelToChoose = levels[rand];
            while (levelToChoose.isCompleted || levelToChoose == currentLevel)
            {
                rand = Random.Range(0, levels.Count);
                levelToChoose = levels[rand];
            }
        }
        
        OnShuffleLevel?.Invoke(this, levelToChoose);
    }

    private void StartLevel(object sender, RoomManagerArgs args)
    {
        OnStartRoom?.Invoke(this, new RoomArgs(args.room));
    }
    
    private void EndLevel(object sender, RoomManagerArgs args)
    {
        OnEndRoom?.Invoke(this, new RoomArgs(args.room));
    }
    
}
