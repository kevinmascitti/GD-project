using System;
using System.Collections;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    [SerializeField] private float flickerInterval = 0.3f; // Intervallo di tempo per alternare visibilità (in secondi)
    private MeshRenderer renderer;
    private bool isFlickering = false;

    void Awake()
    {
        renderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void StartFlick()
    {
        if (!isFlickering)
            StartCoroutine(DisappearAndReappear());
    }

    public void StopFlick()
    {
        isFlickering = false;
    }

    IEnumerator DisappearAndReappear()
    {
        isFlickering = true;
        float elapsedTime = 0f;
        bool visible = true;

        while (isFlickering)
        {
            // Alterna la visibilità
            visible = !visible;
            if (renderer != null)
            {
                renderer.enabled = visible;
            }

            flickerInterval -= 0.01f;
            // Attendi la durata di ogni flicker
            yield return new WaitForSeconds(flickerInterval);
            elapsedTime += flickerInterval;
        }

        if (renderer != null)
        {
            renderer.enabled = true;
        }
    }
    
    
    
}