using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
            GetComponent<Animator>().SetBool("InvalidateMoving",true);
            meleeStateMachine.SetNextState(new GroundEntryState());
        }
        if (Input.GetKeyDown(KeyCode.O) && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState) )
        {
            GetComponent<Animator>().SetBool("InvalidateMoving",true);
            meleeStateMachine.SetNextState(new DashState());
        }
        
        if (Input.GetKeyDown(KeyCode.K) && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState) )
        {
            GetComponent<Animator>().SetBool("InvalidateMoving",true);
            StartCoroutine("validateMoving");
            meleeStateMachine.SetNextState(new KickState());
        }
        
    }
    // soluzione 1: 
    IEnumerator validateMoving()
    {
        Debug.Log("Started Coroutine at timestamp : " + Time.time); 
       yield return new WaitForSeconds(0.5f);
      
       GetComponent<Animator>().SetBool("InvalidateMoving",false);
       Debug.Log("Coroutine ended " + Time.time);
    }
}
