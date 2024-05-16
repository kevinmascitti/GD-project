using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class ComboCounter : MonoBehaviour
{
    [NonSerialized] public int counter = 0;
    
    [SerializeField] private double comboDefaultTimer = 3000;
    private Timer comboTimer;

    public static EventHandler OnCounterIncreased;

    void Start()
    {
        comboTimer = new Timer(comboDefaultTimer);
        comboTimer.Enabled = false;
        comboTimer.AutoReset = false;
        
        EnemyCollision.OnAttackLended += IncreaseCounter;
    }

    private void IncreaseCounter(object sender, EnemyCollisionArgs args)
    {
        comboTimer.Elapsed += ElapsedTimer;
        comboTimer.Start();
        counter += args.comboValue;
        Debug.Log("Combo counter is " + counter);
    }

    private void ElapsedTimer(object sender, ElapsedEventArgs args)
    {
        Debug.Log("STOPPED TIMER");
        counter = 0;
    }
}
