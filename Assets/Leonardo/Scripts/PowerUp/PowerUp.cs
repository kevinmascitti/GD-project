using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PowerUp : MonoBehaviour
{
    public string PUname;

    public float PUduration;

    public float PUvalue;

    public virtual void ActivatePowerUp(object sender, EventArgs args) {}

}
