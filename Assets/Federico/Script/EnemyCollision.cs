using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    public GameObject player;
    
    [SerializeField] private int comboValue;
    
    public static EventHandler<EnemyCollisionArgs> OnAttackLended;

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enemy Hit bro"); 
        TeamComponent enemy= other.gameObject.GetComponent<TeamComponent>();
        
        Animator anim = player.GetComponent<Animator>();
        
        if (enemy != null && anim.GetFloat("Weapon.Active")>0f)
        {
            Debug.Log("Enemy Hit bro");
            OnAttackLended?.Invoke(this, new EnemyCollisionArgs(comboValue));
        }
    }
}

public class EnemyCollisionArgs : EventArgs
{
    public EnemyCollisionArgs(int a)
    {
        comboValue = a;
    }

    public int comboValue;
}