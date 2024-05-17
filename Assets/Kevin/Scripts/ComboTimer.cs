using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboTimer : MonoBehaviour
{
    public double Interval;
    
    [NonSerialized] public bool Enabled;

    private double passedTime;

    static public EventHandler Elapsed;
    
    public ComboTimer(double seconds, bool enabled)
    {
        Interval = seconds;
        Enabled = enabled;
    }

    public void Begin()
    {
        Enabled = false;
        passedTime = Interval;
        Enabled = true;
    }

    public void Stop()
    {
        Enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Enabled)
        {
            passedTime -= Time.deltaTime;
            if (passedTime <= 0)
            {
                Elapsed?.Invoke(this, EventArgs.Empty);
                Enabled = false;
            }
        }
    }
}
