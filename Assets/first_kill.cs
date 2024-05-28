using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class first_kill : Enemy
{
    public GameObject spawner;
    public override void Die()
    {
        base.Die();
        spawner.GetComponent<Spawner>().enabled = true;
    }
}
