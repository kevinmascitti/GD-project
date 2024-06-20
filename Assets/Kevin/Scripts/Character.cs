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
    public float damageReducer = 1;

    private bool isTakingDamage = false;
    private float hurtAnimationDuration;
    private float hurtAnimationStartTime;
    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Damage " + atk);
            if (isPlayer)
            {
                gameObject.GetComponent<PlayerCharacter>().UpdateHP(currentHP-atk);
            }
        }

        if (isTakingDamage)
        {
            if (Time.time >= hurtAnimationStartTime + hurtAnimationDuration)
            {
                isTakingDamage = false;
                //animator.SetTrigger("Walk");
                if (currentHP <= 0)
                    Die();
            }
        }
    }
    
    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage/damageReducer;
        // TO DO animazione danno subito
        GetComponent<Animator>().SetTrigger("TakeDamage");
        AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        hurtAnimationDuration = stateInfo.length;
        hurtAnimationStartTime = Time.time;
        isTakingDamage = true;
        
    }

    public abstract void Die();
}