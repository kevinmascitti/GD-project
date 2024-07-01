using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class LerpAnimation : MonoBehaviour
{
    public float animationSpeed = 50.0f; // La velocità di animazione
    
    private float startTime; // Il tempo iniziale dell'animazione
    private Vector3 startPosition; // La posizione iniziale dell'oggetto
    private Vector3 targetPosition; // La posizione finale desiderata
    [NonSerialized] public bool isAnimating = false; // Flag per controllare se l'animazione è in corso

    public static EventHandler<AnimationArgs> OnEndAnimation;

    // Metodo per iniziare l'animazione
    public void StartAnimation(Vector3 initialPos, Vector3 endingPos)
    {
        startTime = Time.time;
        startPosition = initialPos;
        targetPosition = endingPos;
        isAnimating = true;
    }

    private void Update()
    {
        // Se l'animazione è in corso, esegui il Lerp
        if (isAnimating)
        {
            float elapsedTime = (Time.time - startTime) * animationSpeed;
            float t = elapsedTime / Vector3.Distance(startPosition, targetPosition);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // Se l'oggetto ha raggiunto la posizione finale, interrompi l'animazione
            if (t >= 1.0f)
            {
                isAnimating = false;
                
                OnEndAnimation?.Invoke(this, new AnimationArgs(gameObject));
            }
        }
    }
}

public class AnimationArgs : EventArgs
{
    public AnimationArgs(GameObject obj)
    {
        Obj = obj;
    }

    public GameObject Obj;
}
