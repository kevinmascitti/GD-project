using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PowerUp : MonoBehaviour
{
    public string PUname
    {
        get { return PUname; }
    }

    public float PUduration
    {
        get { return PUduration; }
    }

    public float PUvalue
    {
        get { return PUvalue; }
    }

    public virtual void ActivatePowerUp()
    {
        Debug.Log("PowerUp attivato");
    }

}
