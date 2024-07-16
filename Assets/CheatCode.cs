using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CheatCode : MonoBehaviour
{
    public bool cheat = false;
    private float timeDif = 1.5f;
    private float maxTime = 1.5f;

    public string buffer = "";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeDif -= Time.deltaTime;
        if (timeDif < 0)
        {
            buffer = "";
            
        }

        if (Input.GetKeyDown(KeyCode.C))
            addToBuffer("C");
        if (Input.GetKeyDown(KeyCode.H))
            addToBuffer("H");
        if (Input.GetKeyDown(KeyCode.E))
            addToBuffer("E");
        if (Input.GetKeyDown(KeyCode.A))
            addToBuffer("A");
        if (Input.GetKeyDown(KeyCode.T))
            addToBuffer("T");
        checkPattern();

    }

    private void addToBuffer(string c)
    {
        timeDif = maxTime;
        buffer += c;
    }

    private void checkPattern()
    {
        if (buffer.EndsWith("CHEAT"))
        {
            cheat = true;
            Debug.Log("Trucchi Attivati");
        }
    }

}
