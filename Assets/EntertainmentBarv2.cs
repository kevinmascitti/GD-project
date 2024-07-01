using System;
using UnityEngine;
using UnityEngine.UI;

public class EntertainmentBarv2 : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Gradient gradient;
    
    [Header("Parametri per manipolare la Barra ")]
    [SerializeField] float maxEntertainmentValue = 100;
    [SerializeField] float decreaseSpeed = 10f;
    [SerializeField] private float IncreaseSpeed = 10f;
    [SerializeField] private ComboCounter comboCounter;

    private float currEntertainmentValue = 100;
    private bool isZero = false;
    private bool isActive = false;

    public static EventHandler OnZeroedEnterteinmentBar;

    void Start()
    {
        // Aggiungi un controllo di nullità per fill
        if (fill == null)
        {
            Debug.LogError("L'immagine 'fill' non è assegnata!");
        }
        else
        {
            fill.fillAmount = currEntertainmentValue / maxEntertainmentValue;
        }

        // Aggiungi un controllo di nullità per comboCounter
        if (comboCounter == null)
        {
            Debug.LogError("Il 'comboCounter' non è assegnato!");
        }

        EnemyCollision.OnAttackLended += AttackPerfomed;
        Dash.OnAttackLended += AttackPerfomed;
        PlayerCharacter.OnStartRoom += ResetEntertainmentBar;
        PlayerCharacter.OnStartRoom += StartBar;
        LevelManager.OnStartRoom += ResetEntertainmentBar;
        LevelManager.OnStartRoom += StartBar;
        LevelManager.OnEndRoom += StopBar;
        Room.OnEndRoom += StopBar;
    }

    private void StartBar(object sender, EventArgs args)
    {
        isActive = true;
    }

    private void StopBar(object sender, EventArgs args)
    {
        isActive = false;
    }

    public void IncreaseEnterntaiment()
    {
        if (fill != null)
        {
            fill.fillAmount += 0.2f;
        }
    }

    public void AttackPerfomed(object sender, EventArgs args)
    {
        // Debug.Log("hey la barra d'intrattenimento ha rilevato l'attacco");
        if (isZero)
            isZero = false;

        if (comboCounter != null)
        {
           Debug.Log("ho in crementato il currEntertainmentValue");
            currEntertainmentValue += IncreaseSpeed*comboCounter.counter;
        }
        else
        {
            Debug.LogError("Il 'comboCounter' è null!");
        }

        if (currEntertainmentValue > 100) 
            currEntertainmentValue = 100;

        if (fill != null)
        {
            Debug.Log("ho incrementato il fill amount");
       //     fill.fillAmount += currEntertainmentValue/maxEntertainmentValue;
        }
    }

    public void ChangeEntertainmentBarParameter(float decreaseSpeed, float IncreaseSpeed)
    {
        this.decreaseSpeed = decreaseSpeed;
        this.IncreaseSpeed = IncreaseSpeed;
    }

    public void ResetEntertainmentBar(object sender, EventArgs args)
    {
        currEntertainmentValue = maxEntertainmentValue;
    }

    void Update()
    {
        // TO REMOVE solo ai fini di debugging 
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (comboCounter != null)
            {
                comboCounter.counter += 2;
            }
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

        if (fill != null)
        {
            Debug.Log("ho decrementato il valore del fill");
            fill.fillAmount = currEntertainmentValue / maxEntertainmentValue;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            AttackPerfomed(null, EventArgs.Empty);
        }
    }
}
