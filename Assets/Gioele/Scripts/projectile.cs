using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public int damage = 2;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // il danno del projectile è damage e lo sottraggo alla vita del player
            // vado a diminuire la vita al player che viene colpito 
            other.GetComponent<PlayerCharacter>().currentHP -= 5;
        }

        Destroy(gameObject);
    }

    

    private void Update()
    {
        // fino a che non collide
        transform.position += transform.forward * 3.5f * Time.deltaTime;
        // devo inserire la rotazione
    }
}
