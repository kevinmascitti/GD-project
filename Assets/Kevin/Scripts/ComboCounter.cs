using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class ComboCounter : MonoBehaviour
{
    [NonSerialized] public int counter = 0;
    
    private ComboTimer comboTimer;

    public static EventHandler<int> OnCounterIncreased;
    public static EventHandler OnCounterInitialized;

    void Start()
    {
        comboTimer = GetComponent<ComboTimer>();
        
        EnemyCollision.OnAttackLended += IncreaseCounter;
        Grabbable.OnAttackLended += IncreaseCounter;
        Dash.OnAttackLended += IncreaseCounter;
        ComboTimer.Elapsed += ElapsedTimer;
    }

    private void IncreaseCounter(object sender, EnemyCollisionArgs args)
    {
        comboTimer.Begin();
        counter += args.comboValue;
        OnCounterIncreased?.Invoke(this, counter);
        Debug.Log("Combo counter is " + counter);
    }

    private void ElapsedTimer(object sender, EventArgs args)
    {
        counter = 0;
        OnCounterInitialized?.Invoke(this, EventArgs.Empty);
    }
}
