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
        }

        Destroy(gameObject);
    }
    
}
