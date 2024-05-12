using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class remote_controller : MonoBehaviour
{
    public float timerDuration = 3f;
    public delegate void TimerCallback();

    public LineRenderer laser;
    public float distance = 5f;
    public Transform laserFirePoint;
    public Transform playerTransform;
    void Awake()
    {
        playerTransform = GetComponent<Transform>();
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
        // utilizzo per aprire il menu di pausa
    }
    
    public void VolumePlus()
    {
        // moltiplicatore di combo
    }
    public void VolumeMinus()
    {
        // blocco dall’alto che li “schiaccia” o appiattisce
    }

    public void StartLaser()
    {
        GetComponent<LineRenderer>().enabled = true;
        // raggio laser
        if (Physics.Raycast(playerTransform.position, transform.forward))
        {
            //RaycastHit _raycastHit = Physics.Raycast(playerTransform.position, transform.forward);
            laser.SetPosition(0,laserFirePoint.position);
            laser.SetPosition(1,laserFirePoint.forward*distance);
        }
        
    }
    public void StopLaser()
    {
        // stop raggio laser
        GetComponent<LineRenderer>().enabled = false;
    }

    public void ChPLus()
    {
        //mi ingrandisco (posso colpire più nemici contemporaneamente)
    }
    public void ChMinus()
    {
        //rimpicciolisco i nemici (mi fanno pochi danni)
    }
    public void Pause()
    {
        //nemico bloccato per un tot di tempo (oppure per tutti i nemici???
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            StartLaser();
            StartTimer(timerDuration, StopLaser);
        }
    }
}
