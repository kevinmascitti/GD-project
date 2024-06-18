using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class LifeUpPU : PowerUp {
    private PlayerCharacter playerCharacter;

    public void Start() {
        playerCharacter = GameObject.Find("Player").GetComponent<PlayerCharacter>();
        PUname = "LifeUp";
        PUduration = -1;
        PUvalue = 10;
    }
    public override void ActivatePowerUp(object sender, EventArgs args)
    {
        Debug.Log("LifeUp attivato");
        playerCharacter.UpdateHP(PUvalue);
        PowerUpManager.DisablePowerUp();
    }
}