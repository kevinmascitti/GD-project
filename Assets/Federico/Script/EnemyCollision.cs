using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    [SerializeField] public GameObject player;
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enemy Hit bro"); 
        TeamComponent enemy= other.gameObject.GetComponent<TeamComponent>();
        
        Animator anim = player.GetComponent<Animator>();
        
        if (enemy != null && anim.GetFloat("Weapon.Active")>0f)
        {
            Debug.Log("Enemy Hit bro");
            
        }
    }
}
