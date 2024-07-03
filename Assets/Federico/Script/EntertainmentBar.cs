using System;
using UnityEngine;
using UnityEngine.UI;

public class EntertainmentBar : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Gradient gradient;
    
    [Header("Parametri per manipolare la Barra ")]
    [SerializeField] float maxEntertainmentValue = 100;
    [SerializeField] float decreaseSpeed = 10f;
    [SerializeField] private float IncreaseSpeed = 10f;
    [SerializeField] private ComboCounter comboCounter;

    public float currEntertainmentValue = 100;
    private bool isZero = false;
    private bool isActive = false;

    public static EventHandler OnZeroedEnterteinmentBar;

    void Start()
    {
        if (fill == null)
        {
            Debug.LogError("L'immagine 'fill' non è assegnata!");
        }
        else
        {
            fill.fillAmount = currEntertainmentValue / maxEntertainmentValue;
        }

        if (comboCounter == null)
        {
            Debug.LogError("Il 'comboCounter' non è assegnato!");
        }

        EnemyCollision.OnAttackLended += AttackPerfomed;
        Dash.OnAttackLended += AttackPerfomed;
        Grabbable.OnAttackLended += AttackPerfomed;
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
            currEntertainmentValue += 0.2f * maxEntertainmentValue;
            if (currEntertainmentValue > maxEntertainmentValue)
                currEntertainmentValue = maxEntertainmentValue;
            UpdateFillAmount();
        }
    }

    public void AttackPerfomed(object sender, EventArgs args)
    {
        if (isZero)
            isZero = false;

        if (comboCounter != null)
        {
            currEntertainmentValue += IncreaseSpeed * comboCounter.counter;
        }
        else
        {
            Debug.LogError("Il 'comboCounter' è null!");
        }

        if (currEntertainmentValue > maxEntertainmentValue)
            currEntertainmentValue = maxEntertainmentValue;
        Debug.Log("ho chiamato l'update dell'entertainment bar a seguito di un attacco");
        UpdateFillAmount();
    }

    public void ChangeEntertainmentBarParameter(float decreaseSpeed, float IncreaseSpeed)
    {
        this.decreaseSpeed = decreaseSpeed;
        this.IncreaseSpeed = IncreaseSpeed;
    }

    public void ResetEntertainmentBar(object sender, EventArgs args)
    {
        if (isZero)
        {
            isZero = false;
        }
        currEntertainmentValue = maxEntertainmentValue;
        UpdateFillAmount();
    }

    void Update()
    {
        if (!isZero && isActive)
        {
            currEntertainmentValue -= decreaseSpeed * Time.deltaTime;
            // Debug.Log("ho decrementato il valore del fill");
            if (currEntertainmentValue < 0)
            {
                currEntertainmentValue = 0;
                isZero = true;
                OnZeroedEnterteinmentBar?.Invoke(this, EventArgs.Empty);
            }
            UpdateFillAmount();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            AttackPerfomed(null, EventArgs.Empty);
        }
    }

    private void UpdateFillAmount()
    {
        if (fill != null)
        {
            fill.fillAmount = currEntertainmentValue / maxEntertainmentValue;
            // Debug.Log("UpdateFillAmount: currEntertainmentValue=" + currEntertainmentValue + ", fill.fillAmount=" + fill.fillAmount);

            // Forza l'aggiornamento del canvas
            Canvas.ForceUpdateCanvases();
        }
    }
}
