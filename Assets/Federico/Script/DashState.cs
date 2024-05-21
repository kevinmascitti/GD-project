using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : MeleeBaseState
{
   public override void OnEnter(StateMachine _stateMachine)
   {
      base.OnEnter(_stateMachine);
      duration = 0.7f;
      animator.SetTrigger("Dash");
      Debug.Log("Player Dashed, congratulations!");
   }

   public override void OnUpdate()
   {
      base.OnUpdate();
      if (fixedtime >= duration)
      {
         animator.ResetTrigger("Dash");
         stateMachine.SetNextStateToMain();
      }
   }
}
