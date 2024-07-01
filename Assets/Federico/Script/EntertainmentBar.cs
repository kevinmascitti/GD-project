using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EntertainmentBar : MonoBehaviour
{
    // oggetti della Barra non dovrebbero cambiare mai 
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    [SerializeField] private Gradient gradient;

    /*
     * SIGNIFICATO:
     * MAXENTERTAINMENTVALUE= MASSIMO VALORE DELLA BARRA
     * DECREASESPEED = VELOCITà DI DECRESCITò DELLA BARRA
     * INCREASPEED= VELOCITà DI CRESCITà DELLA BARRA
     * COMBO COUNTER= VALORE CORRENTE DELLA COMBO CHE INFLUENZERà IL QUANTITATIVO DI CRESCITà DELLA BARRA
     * CURRENTERTAINMENTVALUE= VALORE CORRENTE DELLA BARRA QUANDO SI GIOCA
     */
    [Header("Parametri per manipolare la Barra ")]
    [SerializeField] float maxEntertainmentValue = 100;
    [SerializeField] float decreaseSpeed = 10f;
    [SerializeField] private float IncreaseSpeed = 10f;
    // [SerializeField] float comboCounter = 1f;
    [SerializeField] private ComboCounter comboCounter;
    private float currEntertainmentValue = 100;
    private bool isZero = false;
    private bool isActive = false;
    
    private float speed = 1.0f; //how fast it shakes
    private float amount = 1.0f; //how much it shakes

    public static EventHandler OnZeroedEnterteinmentBar;

    // Start is called before the first frame update
    void Start()
    {
        if (slider != null)
        {
            slider.value = currEntertainmentValue / maxEntertainmentValue;
            if (fill != null)
            {
                fill.color = gradient.Evaluate(slider.normalizedValue);
            }
        }

        EnemyCollision.OnAttackLended += AttackPerfomed;
        PlayerCharacter.OnStartRoom += ResetEntertainmentBar;
        PlayerCharacter.OnStartRoom += StartBar;
        LevelManager.OnStartRoom += ResetEntertainmentBar;
        LevelManager.OnStartRoom += StartBar;
        PlayerCharacter.OnEndRoom += StopBar;
        LevelManager.OnEndRoom += StopBar;
        Room.OnEndRoom += StopBar;
        PlayerCharacter.OnNextRoom += ResetEntertainmentBar;
    }

    private void StartBar(object sender, EventArgs args)
    {
        isActive = true;
    }

    private void StopBar(object sender, EventArgs args)
    {
        isActive = false;
    }
    
    //INCREMENTO FLAT DELLA BARRA DI INTRATTENIMENTO TEST 1 
    public void IncreaseEnterntaiment()
    {
        slider.value += 0.2f;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    // FUNZIONE DI ATTACCO EFFETTUATO
    public void AttackPerfomed(object sender, EventArgs args)
    {
        Debug.Log("hey la barra d'intrattenimento ha rilevato l'attacco");
        if(isZero)
            isZero = false;
        currEntertainmentValue += IncreaseSpeed * comboCounter.counter;
        if (currEntertainmentValue > 100) 
            currEntertainmentValue = 100;


        if (slider != null)
        {
            slider.value = currEntertainmentValue / maxEntertainmentValue;
            if (fill != null)
            {
                fill.color = gradient.Evaluate(slider.normalizedValue);
            }
        }
    }

    //QUESTA FUNZIONE VERRà CHIAMATA QUANDO SI ENTRERà IN UNA NUOVA PARTE DEL LIVELLO E CAMBIERà
    // I PARAMETRI DELLA BARRA PER PERMETTERE UNA DECRESCITà MENO RAPIDA A SECONDA DELLE PARTI DELLO SCENARIO
    public void ChangeEntertainmentBarParameter(float decreaseSpeed, float IncreaseSpeed)
    {
        this.decreaseSpeed = decreaseSpeed;
        this.IncreaseSpeed = IncreaseSpeed;
    }

    //RESET DELLA BARRA DI INTRATTENIMENTO A SEGUITO DI UN CAMBIO LIVELLO 
    public void ResetEntertainmentBar(object sender, EventArgs args)
    {
        currEntertainmentValue = maxEntertainmentValue;
    }

    void Update()
    {
        // TO REMOVE solo ai fini di debugging 
        if (Input.GetKeyDown(KeyCode.C))
        {
            comboCounter.counter += 2;
        }

        if (!isZero && isActive)
        {
            currEntertainmentValue -= decreaseSpeed * Time.deltaTime;
            if (currEntertainmentValue < 0)
            {
                currEntertainmentValue = 0;
                isZero = true;
                OnZeroedEnterteinmentBar?.Invoke(this, EventArgs.Empty);
            }
        }

        if (slider != null)
        {
            slider.value = currEntertainmentValue / maxEntertainmentValue;
            if (fill != null)
            {
               fill.color = gradient.Evaluate(slider.normalizedValue);
            }
        }


        if (Input.GetKeyDown(KeyCode.U))
        {
            AttackPerfomed(null, EventArgs.Empty);
        }
    }
}