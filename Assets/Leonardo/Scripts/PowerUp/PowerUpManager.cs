using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpManager : MonoBehaviour {
    [SerializeField] private PowerUp[] powerUpList;
    public static EventHandler OnPowerUpStart;
    public static EventHandler OnPowerUpEnd;
    private static PowerUp currentPowerUp = null;

    private void Start() {
        powerUpList = GetComponentsInChildren<PowerUp>();
    }

    public void SetPowerUp(int powerUpID) {
        if (currentPowerUp != null)
            OnPowerUpStart -= currentPowerUp.ActivatePowerUp;
        currentPowerUp = powerUpList[powerUpID];
        OnPowerUpStart += currentPowerUp.ActivatePowerUp;
    }

    public static void DisablePowerUp() {
        OnPowerUpStart -= currentPowerUp.ActivatePowerUp;
        currentPowerUp = null;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.N)) {
            OnPowerUpStart?.Invoke(this, EventArgs.Empty);
        }
    }


}