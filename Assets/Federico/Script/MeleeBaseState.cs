using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeleeBaseState : State
{
    // How long this state should be active for before moving on
    public float duration;
    // Cached animator component
    protected Animator animator;
    // bool to check whether or not the next attack in the sequence should be played or not
    protected bool shouldCombo;

    
    // The attack index in the sequence of attacks
    protected int attackIndex;
    
    private GameObject HitEffectPrefab;

    private PlayerController controls;
    // Input buffer Timer
    protected float AttackPressedTimer = 0;

    
    
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        animator = GetComponent<Animator>(); 
    //    HitEffectPrefab = GetComponent<ComboCharacterWithDamage>().Hiteffect;
    controls = new PlayerController();
    controls.Gameplay.Attack.performed += ctx => ResetAttackPressedTimer();
    controls.Enable();
    
    }
    void ResetAttackPressedTimer()
    {
        Debug.Log("Ho chiamato la resetAttack da joycon");
        AttackPressedTimer = 2;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        AttackPressedTimer -= Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackPressedTimer = 2;
        }
  
        if (animator.GetFloat("AttackWindow.Open") > 0f && AttackPressedTimer > 0)
        {
            shouldCombo = true;
        }
        else
        {
            shouldCombo = false;
        }
        
    }

  
    public override void OnExit()
    {
        base.OnExit();
    }
/*
    protected void Attack()
    {
        Collider2D[] collidersToDamage = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        int colliderCount = Physics2D.OverlapCollider(hitCollider, filter, collidersToDamage);
        for (int i = 0; i < colliderCount; i++)
        {

            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                TeamComponent hitTeamComponent = collidersToDamage[i].GetComponentInChildren<TeamComponent>();

                // Only check colliders with a valid Team Componnent attached
                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.Enemy)
                {
                    GameObject.Instantiate(HitEffectPrefab, collidersToDamage[i].transform);
                    Debug.Log("Enemy Has Taken:" + attackIndex + "Damage");
                    collidersDamaged.Add(collidersToDamage[i]);
                }
            }
        }
    }
*/
}
