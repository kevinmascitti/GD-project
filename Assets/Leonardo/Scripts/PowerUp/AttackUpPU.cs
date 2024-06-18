using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class AttackUpPU : PowerUp {
    public void Start()
    {
        PUname = "AttackUp";
        PUduration = 10;
        PUvalue = 3;
    }
    public override void ActivatePowerUp(object sender, EventArgs args)
    {
        Debug.Log("AttackUp attivato");
        StartCoroutine(PowerUpDuration());
    }

    private IEnumerator PowerUpDuration()
    {
        yield return new WaitForSeconds(PUduration);
        Debug.Log("AttackUp disattivato");
        PowerUpManager.DisablePowerUp();
    }
}