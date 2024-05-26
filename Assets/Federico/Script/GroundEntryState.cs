using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEntryState : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        attackIndex = 1;
        duration = 0.3f;
        
        animator.SetTrigger("Attack" + attackIndex);
        Debug.Log("Player Attack " + attackIndex + " Fired!");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (fixedtime >= duration)
        {
           
            if (shouldCombo)
            {
                animator.ResetTrigger("Attack"+attackIndex);
                stateMachine.SetNextState(new GroundComboState());
            }
            else
            {
                animator.ResetTrigger("Attack"+attackIndex);
                animator.SetBool("InvalidateMoving",false);
                stateMachine.SetNextStateToMain();
            }
        }
    }
}
