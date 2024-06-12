using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class EnemyAI : Enemy
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
    public float projectileSpeed=20f;
    public float projectileUPSpeed=1.5f;
    // stati
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    // caratteristiche
    [NonSerialized]public bool grounded = true;
    [NonSerialized]public bool OnAttack;
    [SerializeField] private Plane levelPlane;
    
    
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        initialRotation = transform.rotation;
        agent = GetComponent<NavMeshAgent>();
        
    }

    private void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPoint(); //vado a capire dove si trova il player per seguirlo 
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            // evito che vada a seguire un punto per troppo tempo
            StartCoroutine(NuovoObbiettivo(3f));
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 0.3f)// ottengo la distanza effettiva
        {
            walkPointSet = false;
        }
    }  
    IEnumerator NuovoObbiettivo(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SearchWalkPoint();
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
        transform.LookAt(Player.position);
        if (!alreadyAttacked)
        {
            /*
            // attacco ranged/long ranged
            Rigidbody rb = Instantiate(projectile,transform.position,Quaternion.identity).GetComponent<Rigidbody>();
            // trovo lla direzine del player
            Vector3 direction_player = Player.position - transform.position;
            
            rb.AddForce(direction_player.normalized*projectileSpeed,ForceMode.Impulse);
            // se vogliamo che il proiettile sia diretto senza nessun effetto
            // parabolico aumento la forza forward e tolgo la up force
            rb.AddForce(transform.up*projectileUPSpeed,ForceMode.Impulse);
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
    // Update is called once per frame
    void Update()
    {
        if (grounded)
        {
            // solo se l'enemy è a terra
            transform.LookAt(Player.position);
            // vado a vededere se il player se è in attack range o in sight range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
            if (!playerInSightRange && !playerInAttackRange)
                Patroling(); // nulla 
            if (playerInSightRange && !playerInAttackRange)
                ChasePlayer(); // segue il player 
            if (!playerInSightRange && playerInAttackRange && !OnAttack )//&& Math.Abs(this.transform.position.z - Player.transform.position.z) < 0.05f)
                AttackPlayer(); // lo attacca
            transform.rotation = initialRotation;
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



