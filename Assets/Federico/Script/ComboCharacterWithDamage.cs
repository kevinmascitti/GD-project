using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboCharacterWithDamage : MonoBehaviour
{

    private StateMachine meleeStateMachine;

    protected bool shouldKick = false;
   // [SerializeField] public BoxCollider hitbox;
    [SerializeField] public GameObject Hiteffect;

    // Start is called before the first frame update
    void Start()
    {
        meleeStateMachine = GetComponent<StateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            meleeStateMachine.SetNextState(new GroundEntryState());
        }
        if (Input.GetKeyDown(KeyCode.O) && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState) )
        {
            meleeStateMachine.SetNextState(new DashState());
        }
        
        if (Input.GetKeyDown(KeyCode.K) && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState) )
        {
            meleeStateMachine.SetNextState(new KickState());
        }
        
    }
}
