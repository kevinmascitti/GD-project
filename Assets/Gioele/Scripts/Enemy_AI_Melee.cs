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
    public float timeBetweenAttacks;
    public bool alreadyAttacked;
    public GameObject projectile;
    public float projectileSpeed=25f;
    public float projectileUPSpeed=1.2f;
    // stati
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    // caratteristiche
    public int vita; // numero di colpi che può subire prima di morire
    public bool grounded = true;
    [NonSerialized]public bool OnAttack;
    public GameObject levelPlane;
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        initialRotation = transform.rotation;
        agent = GetComponent<NavMeshAgent>();
        levelPlane = GameObject.FindGameObjectWithTag("Spawner").GetComponent<Spawner>().levelPlane;
    }
    

    private void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPoint(); //vado a capire dove si trova il player per seguirlo 
        if (walkPointSet)
            agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 2f)// ottengo la distanza effettiva
        {
            walkPointSet = false;
        }
    }   
        

    private void SearchWalkPoint()
    {
        bool validPointFound = false;

        while (!validPointFound)
        {
            float randomz = Random.Range(-walkPointRange, walkPointRange);
            float randomx = Random.Range(-walkPointRange, walkPointRange);
            walkPoint = new Vector3(Player.transform.position.x + randomx, Player.transform.position.y, Player.transform.position.z + randomz);

            // Controlla se il walkPoint è all'interno dei limiti del piano di livello
            if (walkPoint.x >= levelPlane.transform.position.x && walkPoint.x <= levelPlane.transform.position.x &&
                walkPoint.z >= levelPlane.transform.position.y && walkPoint.z <= levelPlane.transform.position.y)
            {
                // Controlla se il walkPoint è su un terreno valido usando il raycast
                if (Physics.Raycast(walkPoint, -transform.up, 2f, ground))
                {
                    walkPointSet = true;
                    validPointFound = true;
                }
            }
        }
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
            // attacco ranged/long ranged
            Rigidbody rb = Instantiate(projectile,transform.position,Quaternion.identity).GetComponent<Rigidbody>();
            Vector3 direction_player = Player.position - transform.position;
            rb.AddForce(direction_player.normalized*projectileSpeed,ForceMode.Impulse);
            rb.AddForce(transform.up*projectileUPSpeed,ForceMode.Impulse);
            // setto a true perche sto attaccando 
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack),timeBetweenAttacks);// cosi do la temporizzazione per gli attacchi
        }
    }

    private void ResetAttack()
    {
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

            if (!playerInSightRange && playerInAttackRange && !OnAttack)
            {
                    AttackPlayer(); // lo attacca
                    //Debug.Log("chase pplayer");
            }

            transform.rotation = initialRotation;
        }
    }

    public void TakeDamage(int damage)
    {
        vita -= damage;
        if (vita <= 0)
        {
            Invoke(nameof(DestroyEnemy),0.5f);
        }
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    /*
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
            grounded = true;
        }

        
    }
    */

    private void OnDrawGizmosSelected()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position,attackRange);
        Gizmos.color=Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}



