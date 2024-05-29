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

    // EVENTS
    public static EventHandler OnAdStart;

    // PRIVATES
    private Spawner adSpawner;
    private float adTimer = 0f;
    
    private bool hasAdStarted = false;

    private void Start()
    {
        SetSpawnerPlane(null);
        adSpawner = GetComponent<Spawner>();
        TimerBar.maxValue = adDuration;
        TimerBar.value = adDuration;
        OnAdStart += AdSequence;
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

        if(hasAdStarted)
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
                OnAdStart -= AdSequence;
                AdStop();
            }
        }
    }

    private void AdSequence(object sender, EventArgs args)
    {
        hasAdStarted = true;
    }

    private void AdStart()
    {
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
        hasAdStarted = false;
        Destroy(gameObject);
    }

    public void SetSpawnerPlane(GameObject plane)
    {
        adSpawner.levelPlane = plane;
        adSpawner.plane = plane;
    }
}
