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
    void Awake()
    {
        playerTransform = GetComponent<Transform>();
        originalScale = transform.localScale;
        squashAndStress.SetActive(false);
    }
    void MethodToExecute()
    {
        Debug.Log("Il timer è scaduto e ha eseguito il metodo specificato!");
        // Esempio: Disattiva un oggetto
        gameObject.SetActive(false);
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

    public void Menu()
    {
        // utilizzo per aprire il menu di pausa
    }
    public void Mute()
    {
        //muto i nemici urlanti
    }
    public void StartVolumePlus()
    {
        // moltiplicatore di combo
        moltiplicatoreCombo = 1.5f;
        StartTimer(timerDuration, StopVolumePlus);
    }
    public void StopVolumePlus()
    {
        // moltiplicatore di combo
        moltiplicatoreCombo = 1f;
    }
    public void StartVolumeMinus()
    {
        // blocco dall’alto che li “schiaccia” o appiattisce
        StartTimer(timerDuration, StopVolumeMinus);
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
        moltiplicatoreDanniNemici = 1.5f;
        transform.localScale *= 1.5f;
        //mi ingrandisco (posso colpire più nemici contemporaneamente)
        StartTimer(timerDuration,StopChPLus);
    }
    public void StopChPLus()
    {
        moltiplicatoreDanniNemici = 1;
        transform.localScale = originalScale;
        //mi ingrandisco (posso colpire più nemici contemporaneamente)
    }
    public void StartChMinus(string layerName)
    {
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
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            StartLaser();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartPause("Enemy");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartChMinus("Enemy");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartVolumePlus();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartChPLus();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartVolumeMinus();
        }
        
        if (laser.enabled)
        {
            DetectHit();
        }
    }
}
