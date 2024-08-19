using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    { 
            // Blocca e nascondi il cursore quando inizia la scena del gioco
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
    }

    void Update()
    {
        // Puoi aggiungere logica qui per sbloccare il cursore se necessario
        // Per esempio, se premi Esc per aprire un menu di pausa
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = true;
        // }
    }
}
    

 

