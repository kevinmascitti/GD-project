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
    private AnimatorStateInfo stateInfo;
    // Update is called once per frame
    public void Update()
    {
    }
    
    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage/damageReducer;
        // TO DO animazione danno subito
        if (gameObject.tag.CompareTo("Player")!=0)
        {
            GetComponent<Animator>().SetTrigger("TakeDamage");
        }

        isTakingDamage = true;
        if (currentHP <= 0)
            Die();
    }

    public abstract void Die();
}