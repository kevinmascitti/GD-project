using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class Enemy_AI_Melee : Enemy
{
    public NavMeshAgent agent;

    public Transform Player;

    public LayerMask ground, playerLayer;
    // movimento
    public Vector3 walkPoint;

    public bool walkPointSet;

    public float walkPointRange;
    private Quaternion initialRotation;
    // attacco
    private float timeBetweenAttacks;
    public bool alreadyAttacked;

    public float damage;
    // stati
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    // caratteristiche
    public int vita; // numero di colpi che può subire prima di morire
    public bool grounded = true;
    [NonSerialized]public bool OnAttack;
    public Plane levelPlane;
    public Animator _animator;
    private Vector3 forwardVector = new Vector3(-1, 0, 0);
    private Vector3 forwardScaleVector = new Vector3(1, 1, 1);
    private Vector3 backwardVector = new Vector3(1, 0, 0);
    private Vector3 backwardScaleVector = new Vector3(-1, 1, 1);
    
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        initialRotation = transform.rotation;
        agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        timeBetweenAttacks = 0.7f;// frame/framerate corretto

    }
    
    private void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPoint(); //vado a capire dove si trova il player per seguirlo 
        if (walkPointSet)
            agent.SetDestination(walkPoint);
        StartCoroutine(NewTarget(3f));
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 2f)// ottengo la distanza effettiva
        {
            walkPointSet = false;
        }
    }   
    IEnumerator NewTarget(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SearchWalkPoint();
    }

    private void SearchWalkPoint()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        float randomz = Random.Range(-walkPointRange, walkPointRange);
        float randomx = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(Player.transform.position.x + randomx, Player.transform.position.y,
            Player.transform.position.z);
      
        if (Physics.Raycast(walkPoint, -transform.up, 2f, ground))
        {
            walkPointSet = true;
                   
        }
    } 
    bool IsPointOnPlane(Vector3 point)
    {
        // Verifica se il punto è sul piano usando la distanza dal piano
        float distance = levelPlane.GetDistanceToPoint(point);
        return Mathf.Approximately(distance, 0);
    }

    private void ChasePlayer()
    {
        agent.SetDestination(Player.position);
    }
    private void AttackPlayer()
    {
        // controllo che non sia in movimento 
        agent.SetDestination(transform.position);
        //transform.LookAt(Player);
        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            // voglio animazione di attacco 
            _animator.SetBool("attack",true);
            // setto a true perche sto attaccando 
            Invoke(nameof(ResetAttack),timeBetweenAttacks*Time.deltaTime);// cosi do la temporizzazione per gli attacchi
        }
    }

    private void ResetAttack()
    {
        _animator.SetBool("attack",false);
        alreadyAttacked = false;
        // posso attaccare di nuovo 

    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Player"))
        {
            OnAttack = true;
            Invoke("ChangeState", 1.2f);
        }
        if (collision.collider.CompareTag("Ground"))
        {
            //GetComponent<Rigidbody>().isKinematic = true;
            grounded = true;
        }
        
    }
   
    private void ChangeState()
    {
        OnAttack = false;
    }
    /*
     private void OnCollisionStay(Collision collision)
     {

         if (collision.collider.CompareTag("Player"))
         {
             OnAttack = true;
         }
         else
         {
             OnAttack = false;
         }
     }
     */
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         if(Player.transform.position.x > this.transform.position.x){
            //player davanti e enemy dietro
            // Imposta i vettori forward per transform e gunpivot
            transform.forward = forwardVector;
            // Modifica la scala locale
            transform.localScale = forwardScaleVector;
            
        }
        else if (Player.transform.position.x < this.transform.position.x){
            //player davanti e enemy dietro
            // Imposta i vettori forward per transform e gunpivot
            transform.forward = backwardVector;
            // Modifica la scala locale
            transform.localScale = backwardScaleVector;
        }
        if (grounded)// solo se l'enemy è a terra
        {
            // reset rotation
            
            //transform.LookAt(Player.position);
            
            // vado a vededere se il player se è in attack range o in sight range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
            if (!playerInSightRange && !playerInAttackRange)
            {
                Patroling();
                //Debug.Log("patroling"); // nulla 
            }

            if (playerInSightRange && !playerInAttackRange)
            {
                    ChasePlayer(); // segue il player 
                    //Debug.Log("chase pplayer");
            }

            if (!playerInSightRange && playerInAttackRange && !OnAttack && Math.Abs(this.transform.position.z - Player.transform.position.z) < 2f)
            {
                    AttackPlayer(); // lo attacca
                    //Debug.Log("chase pplayer");
            }

            transform.rotation = initialRotation;
        }
    }

   
    
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerCharacter>().TakeDamage(0);
        }

        
    }
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position,attackRange);
        Gizmos.color=Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}



