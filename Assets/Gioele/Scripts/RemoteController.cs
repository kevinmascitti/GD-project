using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RemoteController : MonoBehaviour
{
    public float timerDuration = 3f;
    [SerializeField] private delegate void TimerCallback();
    private Vector3 originalScale;
    private bool isUIVisible = false;
    private bool isStaminaFull = false;
    [SerializeField] private GameObject controllerUI;
    [SerializeField] private float moltiplicatoreCombo = 1f;
    [SerializeField] private float moltiplicatoreDanniNemici = 1f;
    [SerializeField] private LineRenderer laser;
    [SerializeField] private float distance = 5f;
    [SerializeField] private Transform laserFirePoint;
    private Transform playerTransform;
    [NonSerialized] private Vector3 meleeTransform;
    [NonSerialized] private Vector3 rangedTransform;
    [NonSerialized] private Vector3 longRangedTransform;
    [SerializeField] private GameObject squashAndStress;
    [SerializeField] private LayerMask layerLaserMask;
    [SerializeField] private float laserDamage = 0.1f;
    
    // tutti i timer sono attivi non devono ricaricarsi
    private bool RechargeButtonChPlusEnabled = false;
    private bool RechargeButtonVolumePlusEnabled = false;
    private bool RechargeButtonVolumeMinusEnabled = false;
    private bool RechargeButtonLaserEnabled = false;
    private bool RechargeButtonChminusEnabled = false;
    private bool RechargeButtonPauseEnabled = false;
    
    // tutte le abilità sono da sbloccare
    private bool UnlockChPlus = false;
    private bool UnlockVolumePlus = false;
    private bool UnlockVolumeMinus = false;
    private bool UnlockLaser = false;
    private bool UnlockChminus = false;
    private bool UnlockPause = false;

    public static EventHandler OnControllerAbility;

    void Awake()
    {
        playerTransform = GetComponent<Transform>();
        originalScale = transform.localScale;
        if(squashAndStress)
            squashAndStress.SetActive(false);
        controllerUI = GameObject.Find("ControllerUI");
        if(controllerUI)
            controllerUI.SetActive(false);
        isUIVisible = false;
        UnlockChPlus = true;
        UnlockChminus = true;
        UnlockLaser = true;
        UnlockPause = true;
        UnlockVolumeMinus = true;
        UnlockVolumePlus = true;
        PlayerCharacter.OnStaminaFull += SetStaminaFull;
    }

    private void SetStaminaFull(object sender, EventArgs args)
    {
        isStaminaFull = true;
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

    public void RechargeVolumeMinus()
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
            moltiplicatoreCombo = 2f;
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
            StartTimer(2*timerDuration,RechargeVolumeMinus);
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
            if (hit.collider.gameObject.CompareTag("enemy") || hit.collider.gameObject.CompareTag("EnemyObj")){
                // se ho colpito un enemy allora gli faccio un po di danno
                // mantengo il time delta time ? non sono sicuro di questa cosa;
                laserDamage += 0.1f*Time.deltaTime;
                hit.collider.GameObject().GetComponent<Enemy>().currentHP -= 0.1f;
                // non serve credo in laser damage
            }
        }
    }

    public void StartChPlus()
    {
        if (!RechargeButtonChPlusEnabled && UnlockChPlus)
        {
            RechargeButtonChPlusEnabled= true;
            moltiplicatoreDanniNemici = 1.5f;
            transform.localScale *= 1.5f;
            //mi ingrandisco (posso colpire più nemici contemporaneamente)
            StartTimer(timerDuration,StopChPlus);
            StartTimer(2*timerDuration,RechargeChplus);
        }
    }
    public void StopChPlus()
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
            GameObject.FindGameObjectWithTag("Player").GetComponent<Character>().damageReducer =
                moltiplicatoreDanniNemici;
            
            int layer = LayerMask.NameToLayer(layerName);

            // Trova tutti gli oggetti nel layer specificato
            GameObject[] objectsInLayer = GameObject.FindGameObjectsWithTag("enemy");

            // Disattiva il componente NavMeshAgent su ciascun oggetto trovato
            foreach (GameObject obj in objectsInLayer)
            {
                if (obj.GetComponent<Enemy_AI_Melee>())
                {
                    var localScale = obj.transform.localScale;
                    meleeTransform = localScale;
                    localScale *= 0.7f;
                    obj.transform.localScale = localScale;
                }

                if (obj.GetComponent<ShootingTower>()){
                    var localScale = obj.transform.localScale;
                    longRangedTransform = localScale;
                    localScale *= 0.7f;
                    obj.transform.localScale = localScale;
                }

                if (obj.GetComponent<EnemyAI>())
                {
                    var localScale = obj.transform.localScale;
                    rangedTransform = localScale;
                    localScale *= 0.7f;
                    obj.transform.localScale = localScale;
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
        GameObject.FindGameObjectWithTag("Player").GetComponent<Character>().damageReducer =
            moltiplicatoreDanniNemici;
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

            if (obj.GetComponent<ShootingTower>()){
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
                if (obj.GetComponent<Enemy_AI_Melee>())
                {
                    obj.GetComponent<Enemy_AI_Melee>().enabled = false;
                    obj.GetComponent<Animator>().enabled = false;
                }

                if (obj.GetComponent<ShootingTower>())
                {
                    obj.GetComponent<ShootingTower>().enabled = false;
                    obj.GetComponent<Animator>().enabled = false;
                }

                if (obj.GetComponent<EnemyAI>())
                {
                    obj.GetComponent<EnemyAI>().enabled = false;
                    obj.GetComponent<Animator>().enabled = false;
                }

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
            if (obj.GetComponent<Enemy_AI_Melee>())
            {
                obj.GetComponent<Enemy_AI_Melee>().enabled = true;
                obj.GetComponent<Animator>().enabled = true;
            }

            if (obj.GetComponent<ShootingTower>())
            {
                obj.GetComponent<ShootingTower>().enabled = true;
                obj.GetComponent<Animator>().enabled = true;
            }

            if (obj.GetComponent<EnemyAI>())
            {
                obj.GetComponent<EnemyAI>().enabled = true;
                obj.GetComponent<Animator>().enabled = true;
            }
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
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isUIVisible)
        {
            controllerUI.SetActive(true);
            isUIVisible = true;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha1) && isUIVisible)
        {
            controllerUI.SetActive(false);
            isUIVisible = false;
        }

        if (Input.GetKey(KeyCode.Alpha1) && isStaminaFull)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                playerTransform.GetComponent<PlayerCharacter>().UpdateStamina(0);
                StartLaser();
                controllerUI.SetActive(false);
                isUIVisible = false;
                isStaminaFull = false;
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                playerTransform.GetComponent<PlayerCharacter>().UpdateStamina(0);
                StartPause("Enemy");
                controllerUI.SetActive(false);
                isUIVisible = false;
                isStaminaFull = false;
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                playerTransform.GetComponent<PlayerCharacter>().UpdateStamina(0);
                StartChMinus("Enemy");
                controllerUI.SetActive(false);
                isUIVisible = false;
                isStaminaFull = false;
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                playerTransform.GetComponent<PlayerCharacter>().UpdateStamina(0);
                StartVolumePlus();
                controllerUI.SetActive(false);
                isUIVisible = false;
                isStaminaFull = false;
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                playerTransform.GetComponent<PlayerCharacter>().UpdateStamina(0);
                StartChPlus();
                controllerUI.SetActive(false);
                isUIVisible = false;
                isStaminaFull = false;
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                playerTransform.GetComponent<PlayerCharacter>().UpdateStamina(0);
                StartVolumeMinus();
                controllerUI.SetActive(false);
                isUIVisible = false;
                OnControllerAbility?.Invoke(this, EventArgs.Empty);
                isStaminaFull = false;
            }
        }

        if (laser.enabled)
        {
            DetectHit();
        }
    }
}
