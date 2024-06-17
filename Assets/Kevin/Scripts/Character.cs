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
    public float damageReducer=1;

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
    
    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage/damageReducer;
        // TO DO animazione danno subito??
        if (currentHP <= 0)
            Die();
    }

    public abstract void Die();
}