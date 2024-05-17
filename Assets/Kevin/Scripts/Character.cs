using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FixedUpdate = UnityEngine.PlayerLoop.FixedUpdate;

public abstract class Character : MonoBehaviour
{
    public float currentHP;
    public bool isPlayer = false;
    public int atk;

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Damage " + atk);
            if (isPlayer)
            {
                if(currentHP-atk>=0)
                    gameObject.GetComponent<PlayerCharacter>().UpdateHP(currentHP-atk);
                else
                    gameObject.GetComponent<PlayerCharacter>().UpdateHP(0);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        // TO DO animazione danno subito??
    }

    public abstract void Die();
}