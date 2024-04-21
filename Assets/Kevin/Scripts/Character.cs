using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FixedUpdate = UnityEngine.PlayerLoop.FixedUpdate;

public class Character : MonoBehaviour
{
    public float currentHP;
    public bool isPlayer = false;
    public int atk;
    public int def;

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Damage " + atk);
            if (isPlayer)
            {
                if(currentHP-(atk-def)>=0)
                    gameObject.GetComponent<PlayerCharacter>().UpdateHP(currentHP-(atk-def));
                else
                    gameObject.GetComponent<PlayerCharacter>().UpdateHP(0);
            }
        }
    }
}