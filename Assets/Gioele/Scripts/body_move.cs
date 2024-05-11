using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class moving : MonoBehaviour
{
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject punto_esclamativo;
    [SerializeField] private GameObject Shoulder1;
    [SerializeField] private GameObject Arm1;
    [SerializeField] private GameObject MediumArm1;
    [SerializeField] private GameObject UpperArm1;
    [SerializeField] private GameObject Hand1;
    [SerializeField] private GameObject Shoulder2;
    [SerializeField] private GameObject Arm2;
    [SerializeField] private GameObject MediumArm2;
    [SerializeField] private GameObject UpperArm2;
    [SerializeField] private GameObject Hand2;
    [SerializeField] private GameObject nemico;
    private bool timerRunning = false;
    private bool timerRunning2 = false;
    private void Awake()
    {
        punto_esclamativo.SetActive(false);
    }

    public void RotateBy30Degrees(GameObject head)
    {
        head.transform.Rotate(0, 30, 0); // Ruota di 30 gradi rispetto all'asse Y
    }
    
    private void TranslateObjectsOnAxis(float amount)
    {
        TranslateObject(Shoulder1, amount,0,0);
        TranslateObject(Arm1, amount,0,0);
        TranslateObject(MediumArm1, amount,0,0);
        TranslateObject(UpperArm1, amount,0,0);
        TranslateObject(Hand1, amount,0,0);
        
        TranslateObject(Shoulder2, -amount,0,0);
        TranslateObject(Arm2, -amount,0,0);
        TranslateObject(MediumArm2, -amount,0,0);
        TranslateObject(UpperArm2, -amount,0,0);
        TranslateObject(Hand2, -amount,0,0);
    }
    
    private void TranslateObject(GameObject obj, float amountx,float amounty,float amountz)
    {
        // Verifica se l'oggetto è stato assegnato
        if (obj != null)
        {
            // Trasla l'oggetto sull'asse Y
            obj.transform.Translate(amountx, amounty, amountz);
        }
    } 
     public void Shoulders()
     {
         //spallucce
         TranslateObjectsOnAxis(0.3f);
         // parte il timer per fare il contrario
         StartCoroutine(StartTimer(0.5f));
     }
     
     IEnumerator StartTimer(float duration)
     {
         timerRunning = true;
        
         // Aspetta per la durata specificata
         yield return new WaitForSeconds(duration);

         // Chiama il metodo una volta che il tempo è scaduto
         TranslateObjectsOnAxis(-0.3f);
         StartCoroutine(StartTimerForAnimation(0.5f));
        
         timerRunning = false;
     }
     IEnumerator StartTimerForAnimation(float duration)
     {
         timerRunning2 = true;
        
         // Aspetta per la durata specificata
         yield return new WaitForSeconds(duration);

         // Chiama il metodo una volta che il tempo è scaduto
         this.GetComponent<Animator>().enabled = true;

         timerRunning2 = false;
     }

    // Metodo per ruotare di -30 gradi rispetto all'asse Y
    public void RotateByNegative30Degrees(GameObject head)
    {
        head.transform.Rotate(0, -30, 0); // Ruota di -30 gradi rispetto all'asse Y
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            RotateBy30Degrees(head);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            RotateByNegative30Degrees(head);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            punto_esclamativo.SetActive(true);
            nemico.SetActive(true);

        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Shoulders();
        }
    }

}
