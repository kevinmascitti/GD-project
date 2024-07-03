using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTimer : MonoBehaviour
{
    public double Interval;
    private bool Enabled;
    private double passedTime;

    public event Action Elapsed;
    
    public MyTimer(double seconds, bool enabled)
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
                Elapsed?.Invoke();
                Enabled = false;
            }
        }
    }
}
