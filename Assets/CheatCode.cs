using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CheatCode : MonoBehaviour
{
    public bool cheat = false;
    public bool goFast = false;
    public bool entCheat = false;
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
        if (Input.GetKeyDown(KeyCode.F))
            addToBuffer("F");
        if (Input.GetKeyDown(KeyCode.S))
            addToBuffer("S");
        if (Input.GetKeyDown(KeyCode.N))
            addToBuffer("N");
        
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
            cheat = !cheat;
            Debug.Log("Trucchi Attivati");
            
        }
        else if (buffer.EndsWith("FAST"))
        {
            goFast = !goFast;
            Debug.Log("Rapidità Attivata");
        }
        else if (buffer.EndsWith("ENT"))
        {
            entCheat = !entCheat;
            Debug.Log("Trucco barra d'intrattenimento attivato");
        }
        
    }

}
