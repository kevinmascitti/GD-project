using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravity_handler : MonoBehaviour
{
    [SerializeField] private bool grounded;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
            grounded = true;
            Debug.Log("fermo");
            transform.position = new Vector3(transform.position.x, other.transform.position.y, transform.position.z);
        }
    }
}

