using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Constants
{
}

public class Character : MonoBehaviour
{
    public float currentHP;
    public int atk;
    public int def;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Damage " + atk);
            currentHP -= atk - def;
        }

    }
}