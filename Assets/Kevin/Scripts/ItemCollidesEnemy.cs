using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollidesEnemy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && GetComponent<Grabbable>().GetState() == GrabbableState.USED)
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(GetComponent<Grabbable>().GetAtk());
            // VFX colpo
            Destroy(this);
        }
    }
}
