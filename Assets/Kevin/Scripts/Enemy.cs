using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

    public void Update()
    {
        base.Update();
        
    }

    

    public override void Die()
    {
        // TO DO animazione e morte
        Destroy(gameObject);
    }
}
