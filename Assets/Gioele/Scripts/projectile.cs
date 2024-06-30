using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    [NonSerialized] public int damage;
    public float destroyTime = 5.0f;
    private Vector3 direction_player;
    public float speed;
    public void Start()
    {
        speed = 1f;
        StartCoroutine(DestroyAfterDelay(destroyTime));
        transform.Rotate(0, 90, 0, Space.Self);
        //transform.forward = new Vector3(-1, 0, 0);
        direction_player = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // il danno del projectile è damage e lo sottraggo alla vita del player
            // vado a diminuire la vita al player che viene colpito 
            other.GetComponent<PlayerCharacter>().TakeDamage(damage);
            Destroy(gameObject);
        }
        
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        // Aspetta per il tempo specificato
        yield return new WaitForSeconds(delay);
        // Distrugge l'oggetto corrente
        Destroy(gameObject);
    }

    private void Update()
    {
        // spara nella direzione del player
        transform.position += new Vector3(direction_player.x,0,direction_player.z) * speed * Time.deltaTime;
        
    }
}
