using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFinisherState : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        attackIndex = 3;
        duration = 0.4f;
        animator.SetTrigger("Attack" + attackIndex);
        // Debug.Log("Player Attack " + attackIndex + " Fired!");
    }

    public override void OnUpdate()
    {
        
        base.OnUpdate();

        if (fixedtime >= duration)
        {
            animator.ResetTrigger("Attack"+attackIndex);
            animator.SetBool("InvalidateMoving",false);
            stateMachine.SetNextStateToMain();
        }
    }
}
