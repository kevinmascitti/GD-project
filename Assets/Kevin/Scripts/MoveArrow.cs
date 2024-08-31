using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveArrow : MonoBehaviour
{
    // Amplitude of the oscillation (distance from the center)
    public float amplitude = 10.0f;
    
    // Speed of the oscillation
    public float speed = 10.0f;

    // Initial position of the GameObject
    private Vector3 initialPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newX = initialPosition.x + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(newX, initialPosition.y, initialPosition.z);
    }
}
