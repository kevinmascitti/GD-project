using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdTrigger : MonoBehaviour
{ 
    [Header ("Impostazioni della pubblicità")]
    [SerializeField] private float adDuration = 3f;

    [Header ("Oggetti necessari")]
    [SerializeField] private Slider TimerBar;
    [SerializeField] private Canvas PowerUpMenu;

    // EVENTS
    public static EventHandler OnAdStart;
    public static EventHandler OnAdEnd;

    // PRIVATES
    private SpawnerDeterministico adSpawner;
    private float adTimer = 0f;
    
    private bool isAdGoing = false;
    private PlayerCharacter player;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCharacter>();
        adSpawner = GetComponent<SpawnerDeterministico>();
        TimerBar.maxValue = adDuration;
        TimerBar.value = adDuration;
        OnAdStart += AdSequenceStart;
    }

    // Update is called once per frame
    private void Update()
    {
        // DA RIMUOVERE QUANDO SI IMPLEMENTA IL SISTEMA DI TRIGGER
        if(Input.GetKeyDown(KeyCode.LeftAlt)) 
        {
            OnAdStart.Invoke(this, EventArgs.Empty);
        }
        // -------------------------------------------------------

        // Qui c'è tutta la logica alto livello dello svolgimento della scena
        // L'ho messa in Update perché mi serviva il timer col delta time
        if(isAdGoing)
        {
            if(adSpawner.enabled == false)
                AdStart();
            
            if(adTimer <= adDuration)
            {
                adTimer += Time.deltaTime;
                TimerBar.value = adDuration - adTimer;
            }
            else
            {
                OnAdStart -= AdSequenceStart;
                AdStop();
                PowerUpMenu.gameObject.SetActive(true);
                AdSequenceEnd();
            }
        }
    }

    private void AdSequenceStart(object sender, EventArgs args)
    {
        isAdGoing = true;
    }

    private void AdStart()
    {
        SetSpawnerPlane(player.currentRoom.plane);
        adSpawner.enabled = true;
        GetComponent<MeshRenderer>().enabled = false;
        TimerBar.gameObject.SetActive(true);
        Debug.Log("Ad started.");
    }

    private void AdStop() 
    {                
        adSpawner.gameObject.SetActive(false);
        TimerBar.gameObject.SetActive(false);
        // tutti i nemici spawnati vengono eliminati
        GameObject[] enemiesSpawned = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject obj in enemiesSpawned)
        {
            if(obj.GetComponent<Enemy_AI_Melee>())
                obj.GetComponent<Enemy_AI_Melee>().Die();
            if(obj.GetComponent<Enemy_AI_LongRanged>())
                obj.GetComponent<Enemy_AI_LongRanged>().Die();
            if(obj.GetComponent<EnemyAI>())
                obj.GetComponent<EnemyAI>().Die();
        }
        Debug.Log("Ad ended.");
        isAdGoing = false;
        OnAdEnd?.Invoke(this, EventArgs.Empty);
    }

    private void AdSequenceEnd(){
        Destroy(gameObject);
    }

    public void SetSpawnerPlane(GameObject plane)
    {
        adSpawner.levelPlane = plane;
        adSpawner.plane = plane;
    }
}
