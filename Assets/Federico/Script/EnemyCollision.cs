using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    public GameObject player;

    [SerializeField] private int comboValue;

    public static EventHandler<EnemyCollisionArgs> OnAttackLended;
    private GameObject hitEffectPrefabTemp;

    public GameObject HitEffectPrefab;
    /*
    public void OnCollisionEnter(Collision other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        Animator anim = player.GetComponent<Animator>();
        Vector3 contactPoint = other.collider.ClosestPoint(this.transform.position);
      
        
        if (enemy != null && anim.GetFloat("Weapon.Active") > 0f)
        {
            // enemy.TakeDamage(20000.0f);        
            hitEffectPrefabTemp = GameObject.Instantiate(HitEffectPrefab, contactPoint, Quaternion.identity);
            StartCoroutine("KillHitEffect");
            
            // StartCoroutine("KillHitEffect");
            Debug.Log("Enemy Hit");
            OnAttackLended?.Invoke(this, new EnemyCollisionArgs(comboValue));
        }
    }
    */
    public void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        Animator anim = player.GetComponent<Animator>();
        Vector3 contactPoint = other.ClosestPoint(this.transform.position);
    
        if (enemy != null && anim.GetFloat("Weapon.Active") > 0f)
        {
            // Calcola la posizione corretta per l'effetto
            Vector3 effectPosition = other.transform.position + new Vector3(0, +1.5f, 0); // Esempio di offset verticale
        
            hitEffectPrefabTemp = GameObject.Instantiate(HitEffectPrefab, effectPosition, Quaternion.identity);
            StartCoroutine(KillHitEffect(hitEffectPrefabTemp));
            Debug.Log("Enemy Hit");
            OnAttackLended?.Invoke(this, new EnemyCollisionArgs(comboValue));
        }

        enemy = null;
    }


   

    IEnumerator KillHitEffect(GameObject hitEffect)
    {
        yield return new WaitForSeconds(0.2f);

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

