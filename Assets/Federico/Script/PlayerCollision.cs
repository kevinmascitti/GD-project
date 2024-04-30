using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enemy Hit bro"); 
        TeamComponent enemy= other.gameObject.GetComponent<TeamComponent>();
        
        
        
        if (enemy != null )
        {
            Debug.Log("il nemico ti ha colpito bro");
        }
    }
}
