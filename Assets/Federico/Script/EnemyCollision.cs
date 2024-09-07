using System;
using System.Collections;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    public GameObject player;

    [SerializeField] private int comboValue;
    private GameObject hitEffectPrefabTemp;
    public GameObject HitEffectPrefab;
    public ComboCharacterWithDamage oggetto;
    private bool canCollide = true;
    private float collisionCooldown = 0.1f; // Cooldown di 0.1 secondi
    public CheatCode cheatManager;
    public static EventHandler<EnemyCollisionArgs> OnAttackLended;

    [SerializeField] private AudioSource enemyHit;

    void Awake()
    {
       
       
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && cheatManager.cheat )
        {
            OnAttackLended?.Invoke(this, new EnemyCollisionArgs(comboValue));
        }
    }
    
    //
    // public bool CheckCollision(Enemy Enemy,Animator PlayerAnim)
    // {
    //     if (Enemy == null )
    //         return false;
    //     if (PlayerAnim.GetFloat("Weapon.Active") == 0)
    //     {
    //        Debug.Log("collisione ignorata perchè ho disabilitato il weapon active");
    //         return false;
    //     }
    //
    //     if (
    //         (PlayerAnim.GetFloat("LeftArm")>0.0 &&
    //          PlayerAnim.GetFloat("RightArm")==0.0 && 
    //          PlayerAnim.GetFloat("LeftKick")==0.0) || 
    //         (PlayerAnim.GetFloat("LeftArm")==0.0 &&
    //         PlayerAnim.GetFloat("RightArm")>0.0 &&
    //         PlayerAnim.GetFloat("LeftKick")==0.0) ||
    //         (PlayerAnim.GetFloat("LeftArm")==0.0 &&
    //          PlayerAnim.GetFloat("RightArm")==0.0 &&
    //          PlayerAnim.GetFloat("LeftKick")>0.0)
    //         )
    //     {
    //         Debug.Log("Prima collisione trovata disabilito le prossime");
    //         PlayerAnim.SetFloat("Weapon.Active", 0);
    //         return true;
    //     }
        
       // Debug.Log("Error EnemyCollision.cs end of if reached without any return");
        // return false; 
    // }
    public void OnTriggerEnter(Collider other)
    {
        
        
        if (!canCollide) 
        {
            Debug.Log("Collision ignored due to cooldown.");
            return;
        }
        
        

        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        Animator anim = player.GetComponent<Animator>();
        Vector3 contactPoint = other.ClosestPoint(this.transform.position);
        // sostiuire questa riga se l'animator da errore :)
        //enemy!=null && anim.GetFloat("Weapon.Active")>0.0
        if (enemy != null && oggetto!=null && oggetto.isAttacking)
        {
            if(!enemyHit.isPlaying)
                enemyHit.Play();
            
            oggetto.isAttacking = false;
//            Debug.Log("OnTriggerEnter called with: " + other.name + " FROM "+ this);
            Vector3 effectPosition = player.transform.position + player.transform.forward + player.transform.up*0.7f;
            hitEffectPrefabTemp = GameObject.Instantiate(HitEffectPrefab, effectPosition, player.transform.rotation);
            StartCoroutine(KillHitEffect(hitEffectPrefabTemp));
            Debug.Log("Enemy Hit");
            OnAttackLended?.Invoke(this, new EnemyCollisionArgs(comboValue));
            enemy.TakeDamage(1);

            canCollide = false; // Disabilita ulteriori collisioni per la durata del cooldown
            StartCoroutine(CollisionCooldown());
        }
        
        enemy = null;
    }

    IEnumerator CollisionCooldown()
    {
        yield return new WaitForSeconds(collisionCooldown);
        canCollide = true; // Riabilita le collisioni dopo il cooldown
    }

    IEnumerator KillHitEffect(GameObject hitEffect)
    {
        yield return new WaitForSeconds(0.4f);
        GameObject.Destroy(hitEffect);
    }
}

public class EnemyCollisionArgs : EventArgs
{
    public EnemyCollisionArgs(int a)
    {
        comboValue = a;
    }

    public int comboValue;
}