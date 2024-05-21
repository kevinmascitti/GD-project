using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sqash_and_stress_controller : MonoBehaviour
{
    // Start is called before the first frame update
    // The layer index for the "Enemy" layer
    public int enemyLayer;

    void Start()
    {
        // Get the layer index of the "Enemy" layer
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collided object's layer is "Enemy"
        if (other.gameObject.layer == enemyLayer)
        {
            // Destroy the enemy game object
            Destroy(other.gameObject);
            Debug.Log("Enemy destroyed!");
        }
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        // Ottieni le informazioni sullo stato corrente del layer 0 dell'animator
        AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

        

        // Verifica se l'animazione corrente è prossima alla fine
        if (stateInfo.normalizedTime >= 1.0f && !GetComponent<Animator>().IsInTransition(0))
        {
            // Fai qualcosa quando l'animazione è terminata
            this.gameObject.SetActive(false);
            Debug.Log("L'animazione è terminata!");
        }
    }
}
