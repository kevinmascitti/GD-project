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
    
    public void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        Animator anim = player.GetComponent<Animator>();
        Vector3 contactPoint = other.ClosestPoint(this.transform.position);
      
        
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

   

    IEnumerator KillHitEffect()
    {
        Debug.Log("Started Coroutine KillhitEffect  at timestamp : " + Time.time);
        yield return new WaitForSeconds(0.2f);

        GameObject.Destroy(hitEffectPrefabTemp);
        Debug.Log("Coroutine ended " + Time.time);
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

