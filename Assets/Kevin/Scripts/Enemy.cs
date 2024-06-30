using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public static EventHandler OnEnemyDeath;
    
    public void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        // TO DO animazione e morte
        OnEnemyDeath?.Invoke(this,EventArgs.Empty);
        // agigungo la kill al player
        Destroy(gameObject);
    }

}
