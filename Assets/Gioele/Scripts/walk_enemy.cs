using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walk_enemy : MonoBehaviour
{
    // Start is called before the first frame update
    private bool flag=false;
    public float speed = 3f;
    void Start()
    {
        flag = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (flag)
        {
            float translation = speed * Time.deltaTime;
            // Sposta l'oggetto lungo l'asse X
            transform.Translate(translation, 0, 0);
        }
        
    }

}
