using System;
using System.Collections;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    public GameObject player;

    [SerializeField] private int comboValue;
    public static EventHandler<EnemyCollisionArgs> OnAttackLended;
    private GameObject hitEffectPrefabTemp;
    public GameObject HitEffectPrefab;

    private bool canCollide = true;
    private float collisionCooldown = 0.1f; // Cooldown di 0.1 secondi

    public void OnTriggerEnter(Collider other)
    {
        /*
        if (!canCollide) 
        {
            Debug.Log("Collision ignored due to cooldown.");
            return;
        }
        */
        

        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        Animator anim = player.GetComponent<Animator>();
        Vector3 contactPoint = other.ClosestPoint(this.transform.position);

        if (enemy != null && anim.GetFloat("Weapon.Active") > 0f)
        {
            Vector3 effectPosition = other.transform.position + new Vector3(0, +1.5f, 0);
            hitEffectPrefabTemp = GameObject.Instantiate(HitEffectPrefab, effectPosition, Quaternion.identity);
            StartCoroutine(KillHitEffect(hitEffectPrefabTemp));
            Debug.Log("Enemy Hit");
            OnAttackLended?.Invoke(this, new EnemyCollisionArgs(comboValue));

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