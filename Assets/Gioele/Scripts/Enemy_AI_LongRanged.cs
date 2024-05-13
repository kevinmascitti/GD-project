using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class Enemy_AI_LongRanged : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform Player;

    public LayerMask ground, playerLayer;
    // movimento
    public Vector3 walkPoint;

    public bool walkPointSet;
    private Quaternion initialRotation;
    public float walkPointRange;
    // attacco
    [NonSerialized]public float timeBetweenAttacks;
    [NonSerialized]public bool alreadyAttacked;
    public GameObject projectile;
    //public float projectileSpeed=25f;
    //public float projectileUPSpeed=1.2f;
    // stati
    public float sightRange, attackRange;
    [NonSerialized]public bool playerInSightRange, playerInAttackRange;
    // caratteristiche
    public int vita; // numero di colpi che può subire prima di morire
    private bool grounded = false;
    [NonSerialized]public bool OnAttack;
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        initialRotation = transform.rotation;
        agent = GetComponent<NavMeshAgent>();
        
    
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Player"))
        {
            OnAttack = true;
            Invoke("ChangeState", 1.2f);
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
    private void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPoint(); //vado a capire dove si trova il player per seguirlo 
        if (walkPointSet)
            agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)// ottengo la distanza effettiva
        {
            walkPointSet = false;
        }
    }   
        

    private void SearchWalkPoint()
    {
        float randomz = Random.Range(-walkPointRange, walkPointRange);
        float randomx = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(Player.transform.position.x+randomx, Player.transform.position.y, Player.transform.position.z+randomz);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, ground))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(Player.position);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
            grounded = true;
        }

        
    }
    private void AttackPlayer()
    {
        // controllo che non sia in movimento 
        agent.SetDestination(transform.position);
        //transform.LookAt(Player);
        if (!alreadyAttacked)
        {
            /*
            Vector3 directionToPlayer = Player.position - transform.position;
            transform.rotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

            // 3. Istanzia l'oggetto emesso nella posizione e orientazione corrette
            GameObject projectile = Instantiate(this.projectile, transform.position, transform.rotation);
            
            // 4. Aggiungi una forza al proiettile nella direzione del player
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            projectileRb.AddForce(directionToPlayer *projectileSpeed,ForceMode.Impulse);
            projectileRb.AddForce(transform.up*projectileUPSpeed,ForceMode.Impulse);
            // setto a true perche sto attaccando 
            */
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack),timeBetweenAttacks);// cosi do la temporizzazione per gli attacchi
            
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        // posso attaccare di nuovo 

    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (grounded)
        {
            //transform.LookAt(Player);
            // vado a vededere se il player se è in attack range o in sight range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
            if (!playerInSightRange && !playerInAttackRange)
                Patroling(); // nulla 
            if (playerInSightRange && !playerInAttackRange)
                ChasePlayer(); // segue il player 
            if (!playerInSightRange && playerInAttackRange && !OnAttack)
                AttackPlayer(); // lo attacca
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position,attackRange);
        Gizmos.color=Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}



