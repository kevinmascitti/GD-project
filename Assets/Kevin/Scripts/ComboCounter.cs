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

    public static EventHandler OnCounterIncreased;

    void Start()
    {
        comboTimer = GetComponent<ComboTimer>();
        
        EnemyCollision.OnAttackLended += IncreaseCounter;
        ComboTimer.Elapsed += ElapsedTimer;
    }

    private void IncreaseCounter(object sender, EnemyCollisionArgs args)
    {
        comboTimer.Begin();
        counter += args.comboValue;
        OnCounterIncreased?.Invoke(this, EventArgs.Empty);
        Debug.Log("Combo counter is " + counter);
    }

    private void ElapsedTimer(object sender, EventArgs args)
    {
        counter = 0;
    }
}
