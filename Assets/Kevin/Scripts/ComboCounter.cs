using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class ComboCounter : MonoBehaviour
{
    [NonSerialized] public int counter = 0;
    
    private MyTimer comboTimer;

    public static EventHandler<int> OnCounterIncreased;
    public static EventHandler OnCounterInitialized;

    void Start()
    {
        comboTimer = GetComponent<MyTimer>();
        
        EnemyCollision.OnAttackLended += IncreaseCounter;
        Grabbable.OnAttackLended += IncreaseCounter;
        Dash.OnAttackLended += IncreaseCounter;
        PlayerCharacter.OnStartLevel += ResetCounter;
        PlayerCharacter.OnGameOver += ResetCounter;
        comboTimer.Elapsed += ElapsedTimer;
    }

    private void IncreaseCounter(object sender, EnemyCollisionArgs args)
    {
        comboTimer.Begin();
        counter += args.comboValue;
        OnCounterIncreased?.Invoke(this, counter);
        Debug.Log("Combo counter is " + counter);
    }

    private void ElapsedTimer()
    {
        counter = 0;
        OnCounterInitialized?.Invoke(this, EventArgs.Empty);
    }

    private void ResetCounter(object sender, EventArgs args)
    {
        ElapsedTimer();
    }
}
