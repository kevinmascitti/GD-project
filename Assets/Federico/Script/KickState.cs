using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickState : MeleeBaseState
{
    // Start is called before the first frame update
    public float duration=0.2f;

    
    private float KickPressedTimer = 0;

    private GameObject HitEffectPrefab;
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        animator.SetTrigger("Kick");
        
        Debug.Log("Kick button pressed congratulations");
        

    }

    // Update is called once per frame
    public override void  OnUpdate()
    {
        base.OnUpdate();
        if (fixedtime >= duration)
        {
            animator.ResetTrigger("Kick");
            stateMachine.SetNextStateToMain();
        }
     
    }
}
