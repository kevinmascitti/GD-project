using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving : MonoBehaviour
{
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject punto_esclamativo;

    private void Awake()
    {
        punto_esclamativo.SetActive(false);
    }

    public void RotateBy30Degrees(GameObject head)
    {
        head.transform.Rotate(0, 30, 0); // Ruota di 30 gradi rispetto all'asse Y
    }

    // Metodo per ruotare di -30 gradi rispetto all'asse Y
    public void RotateByNegative30Degrees(GameObject head)
    {
        head.transform.Rotate(0, -30, 0); // Ruota di -30 gradi rispetto all'asse Y
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            RotateBy30Degrees(head);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            RotateByNegative30Degrees(head);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            punto_esclamativo.SetActive(true);
            
        }
    }
}
