using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sqash_and_stress_controller : MonoBehaviour
{
    // Start is called before the first frame update
    // The layer index for the "Enemy" layer
    public int enemyLayer;
    private float t = 0;
    public Color red=Color.red;
    public Color green=Color.green;
    public Renderer renderer;
    private static readonly int Color1 = Shader.PropertyToID("_Color");

    void Start()
    {
        // Get the layer index of the "Enemy" layer
        enemyLayer = LayerMask.NameToLayer("Enemy");

    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collided object's layer is "Enemy"
        if (String.Compare(other.gameObject.tag, "enemy", StringComparison.Ordinal)==0 || String.Compare(other.gameObject.tag, "EnemyObj", StringComparison.Ordinal)==0)
        {
            // Destroy the enemy game object
            other.gameObject.GetComponent<Enemy>().currentHP = 0f;
            // da discutere se qui non fare l'animazione
        }
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        t +=  Time.deltaTime;
        Color c=Color.Lerp(green, red, t);
        renderer.material.SetColor(Color1,c);
        // Ottieni le informazioni sullo stato corrente del layer 0 dell'animator
        AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        // Verifica se l'animazione corrente è prossima alla fine
        if (stateInfo.normalizedTime >= 1.0f && !GetComponent<Animator>().IsInTransition(0))
        {
            // Fai qualcosa quando l'animazione è terminata
            this.gameObject.SetActive(false);
        }
    }
}
