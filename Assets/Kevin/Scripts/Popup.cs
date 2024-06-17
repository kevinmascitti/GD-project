using UnityEngine;

public class Popup : MonoBehaviour
{
    public float jumpForce = 5f;
    public float bounceForce = 2f;
    public int maxBounces = 10;

    private Rigidbody rb;
    private int bounceCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground") && bounceCount < maxBounces)
        {
            rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
            bounceCount++;
        }
        else
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}

