using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class Enemy_AI_LongRanged : Enemy
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
    
    // stati
    public float sightRange, attackRange;
    [NonSerialized]public bool playerInSightRange, playerInAttackRange;
    // caratteristiche
    public int vita; // numero di colpi che può subire prima di morire
    private bool grounded = true;
    [NonSerialized]public bool OnAttack;
    public Plane levelPlane;
    // VECTOR CONSTANTS TO ROTATE THE PLAYER
    private Vector3 forwardVector = new Vector3(-1, 0, 0);
    private Vector3 forwardScaleVector = new Vector3(1, 1, 1);
    private Vector3 backwardVector = new Vector3(1, 0, 0);
    private Vector3 backwardScaleVector = new Vector3(-1, 1, 1);
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
    private void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPoint(); //vado a capire dove si trova il player per seguirlo 
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            StartCoroutine(NewTarget(3f));
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)// ottengo la distanza effettiva
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
        bool pointFound = false;
        while (!pointFound)
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            //float randomz = Random.Range(-walkPointRange, walkPointRange);
            float randomx;
            if (Random.value < 0.5f)
            {
                // Genera un valore casuale tra walkpoint range e -6
                randomx = Random.Range(-walkPointRange, -6f);
            }
            else
            {
                // Genera un valore casuale tra walkpoint range e 8
                randomx = Random.Range(6f, walkPointRange);
            }
            walkPoint = new Vector3(Player.transform.position.x + randomx, Player.transform.position.y,
                Player.transform.position.z);
            pointFound=IsPointOnPlane(walkPoint);
        }

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
    /*
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
            grounded = true;
        }

        
    }*/
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
        float range= 4f;
        float playerX = Player.transform.position.x;
        float targetX = transform.position.x;
    
        // Controlla se targetX è all'interno dell'intervallo playerX ± range
        if(targetX >= (playerX - range) && targetX <= (playerX + range)){
            // ho enemuy nella posizione sbagliata cerco un nuovo punto
            SearchWalkPoint();
        }
        if(Player.transform.position.x > this.transform.position.x){
            //player davanti e enemy dietro
            transform.forward = forwardVector;
            transform.localScale = forwardScaleVector;
        }
        else if (Player.transform.position.x < this.transform.position.x){
            //player dietro e enemy davanti
            transform.forward = backwardVector;
            transform.localScale = backwardScaleVector;
        }
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
            if (!playerInSightRange && playerInAttackRange && !OnAttack && Math.Abs(this.transform.position.z - Player.transform.position.z) < 0.05f)
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



