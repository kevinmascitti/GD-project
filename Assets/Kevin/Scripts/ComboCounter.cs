using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class ComboCounter : MonoBehaviour
{
    [NonSerialized] public int counter = 0;
    
    [SerializeField] private double comboDefaultTimer = 3.0;
    private Timer comboTimer;

    public static EventHandler OnCounterIncreased;

    void Start()
    {
        comboTimer.Enabled = true;
        comboTimer.Interval = comboDefaultTimer;
        
        comboTimer.Elapsed += ElapsedTimer;
        EnemyCollision.OnAttackLended += IncreaseCounter;
    }

    private void IncreaseCounter(object sender, EnemyCollisionArgs args)
    {
        comboTimer.Start();
        counter += args.comboValue;

    }

    private void ElapsedTimer(object sender, ElapsedEventArgs args)
    {
        comboTimer.Stop();
        counter = 0;
    }
}
