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
    public float timeBetweenAttacks;
    [NonSerialized]public bool alreadyAttacked;
    public GameObject projectile;
    public float projectileSpeed=20f;
    public float projectileUPSpeed=1.5f;

    public Transform gunpivot;
    // stati
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    // caratteristiche
    [NonSerialized]public bool grounded = true;
    [NonSerialized]public bool OnAttack;
    [SerializeField] private Plane levelPlane;
    // VECTOR CONSTANTS TO ROTATE THE PLAYER
    private Vector3 forwardVector = new Vector3(-1, 0, 0);
    private Vector3 forwardScaleVector = new Vector3(1, 1, 1);
    private Vector3 backwardVector = new Vector3(1, 0, 0);
    private Vector3 backwardScaleVector = new Vector3(1, 1, -1);
    private bool destinationReached = false;
    private int sign;
    
    
    private void Awake()
    {
        timeBetweenAttacks = 0.58f;
        if (gameObject.name.CompareTo("sushiChefUnriggedKnife") == 0)
        {
            // temporizzazione chef
            timeBetweenAttacks = 1.04f;
        }

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
            StartCoroutine(NewTarget(4f));
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 0.3f)// ottengo la distanza effettiva
        {
            walkPointSet = false;
        }
    }  
    IEnumerator NewTarget(float seconds)
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
       //float randomz = Random.Range(-walkPointRange, walkPointRange);
       float randomx;
       float randomz;
       if (Random.value < 0.5f)
       {
           // Genera un valore casuale tra walkpoint range e -6
           randomx = Random.Range(-walkPointRange, -6f);
           randomz = Random.Range(-walkPointRange, -4f);
       }
       else
       {
           // Genera un valore casuale tra walkpoint range e 8
           randomx = Random.Range(6f, walkPointRange);
           randomz = Random.Range(-walkPointRange, -4f);
       }
       walkPoint = new Vector3(Player.transform.position.x + randomx, Player.transform.position.y,
               Player.transform.position.z+randomz);
      
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
        GetComponent<Animator>().SetBool("shot",true);
        // controllo che non sia in movimento 
        agent.SetDestination(transform.position);
        transform.LookAt(Player.position);
        if (!alreadyAttacked )
        {
            // attacco ranged/long ranged
            if (sign == -1)
            {
                GameObject rb = Instantiate(projectile, new Vector3(gunpivot.transform.position.x-1.5f,gunpivot.transform.position.y,gunpivot.transform.position.z+0.4f), Quaternion.identity);
                // trovo lla direzine del player
                Vector3 direction_player = Player.position - gunpivot.position;
                rb.transform.forward = new Vector3(direction_player.x,direction_player.y+1.5f,direction_player.z);
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack),timeBetweenAttacks);// cosi do la temporizzazione per gli attacchi
            }
            else
            {
                GameObject rb = Instantiate(projectile, gunpivot.transform.position, Quaternion.identity);
                // trovo lla direzine del player
                Vector3 direction_player = Player.position - gunpivot.position;
                rb.transform.forward = new Vector3(direction_player.x,direction_player.y+1.5f,direction_player.z);
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack),timeBetweenAttacks);// cosi do la temporizzazione per gli attacchi
            }
            
            
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        GetComponent<Animator>().SetBool("shot",false);
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
        float range= 4f;
        float playerX = Player.transform.position.x;
        float targetX = transform.position.x;
    
        // Controlla se targetX è all'interno dell'intervallo playerX ± range
        if(targetX >= (playerX - range) && targetX <= (playerX + range)){
            // ho enemuy nella posizione sbagliata cerco un nuovo punto
            Patroling();
        }
        if(Player.transform.position.x > this.transform.position.x){
            //player davanti e enemy dietro
            // Imposta i vettori forward per transform e gunpivot
            transform.forward = forwardVector;
            gunpivot.forward = forwardVector;
            sign = 1;
            // Modifica la scala locale
            transform.localScale = forwardScaleVector;
            gunpivot.localScale = forwardScaleVector;

            // Salva la posizione originale
            Vector3 originalPosition = gunpivot.position;

            // Salva l'offset originale locale dell'oggetto rispetto al suo genitore
            Vector3 localPositionOffset = gunpivot.localPosition;

            // Calcola l'offset dopo l'inversione della scala
            Vector3 newLocalPositionOffset = Vector3.Scale(localPositionOffset, forwardScaleVector);

            // Modifica la scala dell'oggetto
            gunpivot.localScale = Vector3.Scale(gunpivot.localScale, forwardScaleVector);

            // Calcola la differenza di posizione
            Vector3 positionOffset = gunpivot.parent.TransformPoint(newLocalPositionOffset) - gunpivot.parent.TransformPoint(localPositionOffset);

            // Applica la differenza alla posizione originale
            gunpivot.position = originalPosition + positionOffset;
        }
        else if (Player.transform.position.x < this.transform.position.x){
            //player davanti e enemy dietro
            // Imposta i vettori forward per transform e gunpivot
            transform.forward = backwardVector;
            gunpivot.forward = backwardVector;
            sign = -1;
            // Modifica la scala locale
            transform.localScale = backwardScaleVector;
            gunpivot.localScale = backwardScaleVector;

            // Salva la posizione originale
            Vector3 originalPosition = gunpivot.position;

            // Salva l'offset originale locale dell'oggetto rispetto al suo genitore
            Vector3 localPositionOffset = gunpivot.localPosition;

            // Calcola l'offset dopo l'inversione della scala
            Vector3 newLocalPositionOffset = Vector3.Scale(localPositionOffset, backwardScaleVector);

            // Modifica la scala dell'oggetto
            gunpivot.localScale = Vector3.Scale(gunpivot.localScale, backwardScaleVector);

            // Calcola la differenza di posizione
            Vector3 positionOffset = gunpivot.parent.TransformPoint(newLocalPositionOffset) - gunpivot.parent.TransformPoint(localPositionOffset);

            // Applica la differenza alla posizione originale
            gunpivot.position = originalPosition + positionOffset;
        }
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
                //GetComponent<Animator>().SetBool("walking",true);
            if (!playerInSightRange && playerInAttackRange && !OnAttack && HasArrived())// && Math.Abs(this.transform.position.z - Player.transform.position.z) < 0.05f)
                AttackPlayer();
                // lo attacca
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
    
    private bool HasArrived()
    {
        // Controlla se l'agente ha una destinazione
        if (!agent.pathPending)
        {
            // Controlla se l'agente è vicino alla destinazione
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // Controlla se l'agente non si sta più muovendo
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }
}



