using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
public class ComboCharacterWithDamage : MonoBehaviour
{

    private StateMachine meleeStateMachine;
    private GameObject hitEffectPrefabTemp;
    protected bool shouldKick = false;
   // [SerializeField] public BoxCollider hitbox;
    public GameObject HitEffectPrefab;
    // Start is called before the first frame update
    public bool isAttacking = false;
    public GameObject parentObject; // Assegna l'oggetto padre via Inspector
    public string boneName; // Nome dell'osso che vuoi trovare
    public GameObject handBone;
    private PlayerController controls;
    public static EventHandler OnDashRequested;
    public static EventHandler<float> OnDashLaunched;

    private void OnEnable()
    {
        //Debug.Log("OnEnable: Enabling controls");
        controls.Enable();
    }
    
    private void Awake()
    {
        controls = new PlayerController();
        controls.Gameplay.Attack.performed += ctx => LaunchFirstAttack();
        
    }

    void Start()
    {
        meleeStateMachine = GetComponent<StateMachine>();
        Transform[] childTransforms = parentObject.GetComponentsInChildren<Transform>();

        Transform targetBone = null;
        foreach (Transform childTransform in childTransforms)
        {
            if (childTransform.name == boneName)
            {
                targetBone = childTransform;
                break;
            }
        }

        if (targetBone != null)
        {
            Debug.Log("Osso trovato: " + targetBone.name);
            // Ora puoi fare qualcosa con l'osso trovato, ad esempio:
            // targetBone.position = new Vector3(0, 0, 0);
        }
        else
        {
            Debug.Log("Osso non trovato.");
        }

        Dash.OnCheckedDash += LaunchDash;
        Dash.OnDashEnded += ValidateMovingNow;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PlayerCharacter>().isInputEnabled)
        {
            if (Input.GetMouseButton(0) && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
            {
                LaunchFirstAttack();
                
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
            {
                OnDashRequested?.Invoke(this, EventArgs.Empty);
            }

            if (Input.GetKeyDown(KeyCode.K) && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
            {
                GetComponent<Animator>().SetBool("InvalidateMoving", true);
                StartCoroutine(ValidateMoving(0.5f));
                meleeStateMachine.SetNextState(new KickState());
            }
        }
    }

     void LaunchFirstAttack()
     {
         Debug.Log("ho lanciato il primo attacco");
        isAttacking = true;
        Vector3 contactPoint = handBone.transform.position;
        GetComponent<Animator>().SetBool("InvalidateMoving", true);
      //  controls.Disable();
        meleeStateMachine.SetNextState(new GroundEntryState());
  
    }
    private void ValidateMovingNow(object sender, EventArgs args)
    {
        StartCoroutine(ValidateMoving(0));
    }

    IEnumerator ValidateMoving(float validationTime)
    { 
       yield return new WaitForSeconds(validationTime);
      
       GetComponent<Animator>().SetBool("InvalidateMoving",false);
    }
    IEnumerator KillHitEffect()
    {
        Debug.Log("Started Coroutine KillhitEffect  at timestamp : " + Time.time);
        yield return new WaitForSeconds(0.1f);

        GameObject.Destroy(hitEffectPrefabTemp);
    }

    private void LaunchDash(object sender, float distance)
    {
        GetComponent<Animator>().SetBool("InvalidateMoving", true);
        meleeStateMachine.SetNextState(new DashState());
        OnDashLaunched?.Invoke(this, distance);
    }

}
