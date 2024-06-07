using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class remote_controller : MonoBehaviour
{
    public float timerDuration = 3f;
    [SerializeField]private delegate void TimerCallback();
    private Vector3 originalScale;
    [SerializeField]private float moltiplicatoreCombo = 1f;
    [SerializeField]private float moltiplicatoreDanniNemici = 1f;
    [SerializeField]private LineRenderer laser;
    [SerializeField]private float distance = 5f;
    [SerializeField]private Transform laserFirePoint;
    [SerializeField]private Transform playerTransform;
    [NonSerialized]private Vector3 meleeTransform;
    [NonSerialized]private Vector3 rangedTransform;
    [NonSerialized]private Vector3 longRangedTransform;
    [SerializeField] private GameObject squashAndStress;
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask layerLaserMask;
    [SerializeField] private float laserDamage = 0.1f;
    [SerializeField] public bool RechargeButtonChPlusEnabled;
    [SerializeField] public bool RechargeButtonVolumePlusEnabled;
    [SerializeField] public bool RechargeButtonVolumeMinusEnabled;
    [SerializeField] public bool RechargeButtonLaserEnabled;
    [SerializeField] public bool RechargeButtonChminusEnabled;
    [SerializeField] public bool RechargeButtonPauseEnabled;
    [SerializeField] private bool UnlockChPlus;
    [SerializeField] private bool UnlockVolumePlus;
    [SerializeField] private bool UnlockVolumeMinus;
    [SerializeField] private bool UnlockLaser;
    [SerializeField] private bool UnlockChminus;
    [SerializeField] private bool UnlockPause;
    void Awake()
    {
        playerTransform = GetComponent<Transform>();
        originalScale = transform.localScale;
        squashAndStress.SetActive(false);
        // tutti i timer sono attivi non devono ricaricarsi
        RechargeButtonLaserEnabled = false;
        RechargeButtonChminusEnabled = false;
        RechargeButtonPauseEnabled = false;
        RechargeButtonVolumeMinusEnabled = false;
        RechargeButtonVolumePlusEnabled = false;
        RechargeButtonChPlusEnabled = false;
        // tutte le abilità sono da sbloccare
        UnlockChPlus=false;
        UnlockVolumePlus=false;
        UnlockVolumeMinus=false;
        UnlockLaser=false;
        UnlockChminus=false;
        UnlockPause=false;
    }
    // enable dei bottoni
    public void UnlockChPlusButton()
    {
        UnlockChPlus = true;
    }
    public void UnlockVolumePlusButton()
    {
        UnlockVolumePlus = true;
    }
    public void UnlockLaserButton()
    {
        UnlockLaser = true;
    }
    public void UnlockChminusButton()
    {
        UnlockChminus = true;
    }
    public void UnlockPauseButton()
    {
        UnlockPause = true;
    }
    public void UnlockVolumeMinusButton()
    {
        UnlockVolumeMinus = true;
    }
    // ricarica bottoni
    
    public void RechargedChplus()
    {
        RechargeButtonChPlusEnabled = true;
    }
    public void RechargeChplus()
    {
        RechargeButtonChPlusEnabled = false;
    }

    public void RechargedChminus()
    {
        RechargeButtonChminusEnabled = true;
    }

    public void RechargeChminus()
    {
        RechargeButtonChminusEnabled = false;
    }
    public void RechargedPause()
    {
        RechargeButtonPauseEnabled = true;
    }

    public void RechargePause()
    {
        RechargeButtonPauseEnabled = false;
    }
    public void RechargedVolumeplus()
    {
        RechargeButtonVolumePlusEnabled = true;
    }

    public void RechargeVolumePlus()
    {
        RechargeButtonVolumePlusEnabled = false;
    }
    public void RechargedVolumeMinus()
    {
        RechargeButtonVolumeMinusEnabled = true;
    }

    public void RechargeVolumeMInus()
    {
        RechargeButtonVolumeMinusEnabled = false;
    }
    public void RechargedLaser()
    {
        RechargeButtonLaserEnabled = true;
    }

    public void RechargeLaser()
    {
        RechargeButtonLaserEnabled = false;
    }
    


    // Metodo per avviare il timer
    void StartTimer(float duration, TimerCallback callback)
    {
        StartCoroutine(TimerCoroutine(duration, callback));
    }

    // Coroutine per il timer
    IEnumerator TimerCoroutine(float duration, TimerCallback callback)
    {
        yield return new WaitForSeconds(duration);
        // Esegui il metodo specificato
        callback();
    }
    
    public void Mute()
    {
        //muto i nemici urlanti
    }
    public void StartVolumePlus()
    {
        if (!RechargeButtonVolumePlusEnabled && UnlockVolumePlus)
        {
            RechargeButtonVolumePlusEnabled = true;
            // moltiplicatore di combo
            moltiplicatoreCombo = 1.5f;
            StartTimer(timerDuration, StopVolumePlus);
            StartTimer(2*timerDuration,RechargeVolumePlus);
        }
    }
    
    public void StopVolumePlus()
    {
        
        // moltiplicatore di combo
        moltiplicatoreCombo = 1f;
            
    }
    public void StartVolumeMinus()
    {
        if (!RechargeButtonVolumeMinusEnabled && UnlockVolumeMinus)
        {
            RechargeButtonVolumeMinusEnabled = true;
            // blocco dall’alto che li “schiaccia” o appiattisce
            StartTimer(timerDuration, StopVolumeMinus);
            StartTimer(2*timerDuration,RechargeVolumeMInus);
        }
    }
    public void StopVolumeMinus()
    {
        // blocco dall’alto che li “schiaccia” o appiattisce
        squashAndStress.SetActive(true);
        //Vector3 newPosition = player.transform.position;

        // Set the position of squashAndStress
        //squashAndStress.transform.position = newPosition;
    }

    public void StartLaser()
    {
        if (!RechargeButtonLaserEnabled && UnlockLaser)
        {
            RechargeButtonLaserEnabled = true;
            GetComponent<LineRenderer>().enabled = true;
            // raggio laser
            if (Physics.Raycast(playerTransform.position, transform.forward))
            {
                // non ho idea del motivo ma se setto una nuova posizione funziona malissimo, 
                
                // utilizziamo i parametri dati da console 
                
                //RaycastHit _raycastHit = Physics.Raycast(playerTransform.position, transform.forward);
                //laser.SetPosition(0,laserFirePoint.position);
                //laser.SetPosition(1,new Vector3(laserFirePoint.position.x,laserFirePoint.position.y, laserFirePoint.position.z+distance));
            }
            StartTimer(timerDuration, StopLaser);
            StartTimer(2*timerDuration,RechargeLaser);
        }
    }
    public void StopLaser()
    {
        // stop raggio laser
        GetComponent<LineRenderer>().enabled = false;
    }

    void DetectHit()
    {
        // Posizione di partenza del raycast
        Vector3 startPoint = laserFirePoint.position;
        // Direzione del raycast (dritta lungo l'asse in avanti del laserFirePoint)
        Vector3 direction = laserFirePoint.forward;

        // Visualizza il raycast con una linea blu
        Debug.DrawRay(startPoint, direction * 2f, Color.blue);

        // Esegui il raycast
        RaycastHit hit;
        if (Physics.Raycast(startPoint, direction, out hit, 2f))
        {
            // Se c'è una collisione, fai qualcosa con l'oggetto colpito
            if (hit.collider.gameObject.CompareTag("enemy"))
            {
                // se ho colpito un enemy allora gli faccio un po di danno
                // mantengo il time delta time ? non sono sicuro di questa cosa;
                laserDamage += 0.1f*Time.deltaTime;
                hit.collider.GameObject().GetComponent<Enemy>().currentHP -= 0.1f;
                // non serve credo in laser damage
            }
        }
    }

    public void StartChPLus()
    {
        if (!RechargeButtonChPlusEnabled && UnlockChPlus)
        {
            RechargeButtonChPlusEnabled= true;
            moltiplicatoreDanniNemici = 1.5f;
            transform.localScale *= 1.5f;
            //mi ingrandisco (posso colpire più nemici contemporaneamente)
            StartTimer(timerDuration,StopChPLus);
            StartTimer(2*timerDuration,RechargeChplus);
        }
    }
    public void StopChPLus()
    {
        moltiplicatoreDanniNemici = 1;
        transform.localScale = originalScale;
        //mi ingrandisco (posso colpire più nemici contemporaneamente)
    }
    public void StartChMinus(string layerName)
    {
        // se il bottone è sbloccato e non si sta ricaricando
        if (!RechargeButtonChminusEnabled && UnlockChminus)
        {
            RechargeButtonChminusEnabled= true;
            moltiplicatoreDanniNemici = 1.5f;
            
            int layer = LayerMask.NameToLayer(layerName);

            // Trova tutti gli oggetti nel layer specificato
            GameObject[] objectsInLayer = GameObject.FindGameObjectsWithTag("enemy");

            // Disattiva il componente NavMeshAgent su ciascun oggetto trovato
            foreach (GameObject obj in objectsInLayer)
            {
                if (obj.GetComponent<Enemy_AI_Melee>())
                {
                    meleeTransform = obj.transform.localScale;
                    obj.transform.localScale *= 0.7f;
                }

                if (obj.GetComponent<Enemy_AI_LongRanged>()){
                    longRangedTransform = obj.transform.localScale;
                    obj.transform.localScale *= 0.7f;
                }

                if (obj.GetComponent<EnemyAI>())
                {
                    rangedTransform = obj.transform.localScale;
                    obj.transform.localScale *= 0.7f;
                }

                
            }
            //rimpicciolisco i nemici (mi fanno pochi danni)
            StartTimer(timerDuration,StopChMinus);
            StartTimer(2*timerDuration,RechargeChminus);
        }
    }
    public void StopChMinus()
    {
        moltiplicatoreDanniNemici = 1f;
        int layer = LayerMask.NameToLayer("Enemy");

        // Trova tutti gli oggetti nel layer specificato
        GameObject[] objectsInLayer = GameObject.FindGameObjectsWithTag("enemy");

        // Disattiva il componente NavMeshAgent su ciascun oggetto trovato
        foreach (GameObject obj in objectsInLayer)
        {
            if (obj.GetComponent<Enemy_AI_Melee>())
            {
                obj.transform.localScale=meleeTransform;
                // torna alla local scale orginale
            }

            if (obj.GetComponent<Enemy_AI_LongRanged>()){
                obj.transform.localScale=longRangedTransform ;
            }

            if (obj.GetComponent<EnemyAI>())
            {
                obj.transform.localScale=rangedTransform;
            }

            
        }
        //rimpicciolisco i nemici (mi fanno pochi danni)

    }
    public void StartPause(string layerName)
    {
        if (!RechargeButtonPauseEnabled && UnlockPause)
        {
            RechargeButtonPauseEnabled = true;
            // Ottieni il numero di layer dall'indice o dal nome
            int layer = LayerMask.NameToLayer(layerName);

            // Trova tutti gli oggetti nel layer specificato
            GameObject[] objectsInLayer = GameObject.FindGameObjectsWithTag("enemy");

            // Disattiva il componente NavMeshAgent su ciascun oggetto trovato
            foreach (GameObject obj in objectsInLayer)
            {
                if(obj.GetComponent<Enemy_AI_Melee>())
                    obj.GetComponent<Enemy_AI_Melee>().enabled = false;
                if(obj.GetComponent<Enemy_AI_LongRanged>())
                    obj.GetComponent<Enemy_AI_LongRanged>().enabled = false;
                if(obj.GetComponent<EnemyAI>())
                    obj.GetComponent<EnemyAI>().enabled = false;
                NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.enabled = false;
                }
            }
            StartTimer(timerDuration,StopPause);
            StartTimer(2*timerDuration,RechargePause);
        }
    }

    public void StopPause()
    {
        int layer = LayerMask.NameToLayer("Enemy");

        // Trova tutti gli oggetti nel layer specificato
        GameObject[] objectsInLayer = GameObject.FindGameObjectsWithTag("enemy");

        // Disattiva il componente NavMeshAgent su ciascun oggetto trovato
        foreach (GameObject obj in objectsInLayer)
        {
            if(obj.GetComponent<Enemy_AI_Melee>())
                obj.GetComponent<Enemy_AI_Melee>().enabled = true;
            if(obj.GetComponent<Enemy_AI_LongRanged>())
                obj.GetComponent<Enemy_AI_LongRanged>().enabled = true;
            if(obj.GetComponent<EnemyAI>())
                obj.GetComponent<EnemyAI>().enabled = true;
            NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.I)  )
        {
            StartLaser();
        }
        if (Input.GetKeyDown(KeyCode.U) )
        {
            StartPause("Enemy");
        }
        if (Input.GetKeyDown(KeyCode.O) )
        {
            StartChMinus("Enemy");
        }
        if (Input.GetKeyDown(KeyCode.P) )
        {
            StartVolumePlus();
        }
        if (Input.GetKeyDown(KeyCode.L) )
        {
            StartChPLus();
        }
        if (Input.GetKeyDown(KeyCode.K) )
        {
            StartVolumeMinus();
        }
        
        if (laser.enabled)
        {
            DetectHit();
        }
    }
}
