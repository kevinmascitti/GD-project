using UnityEngine;

public class AntiOverlap : MonoBehaviour
{
    public float detectionRadius = 1.0f; // Raggio per i raggi
    public int numRays = 8; // Numero di raggi da lanciare per ogni livello
    public float pushForce = 10f; // Forza con cui spingere indietro i nemici
    public float[] heights = new float[] { 1.0f }; // Altezze da cui lanciare i raggi
    public LayerMask enemyLayer; // Layer dei nemici

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Il componente Rigidbody non è presente sul giocatore.");
        }
    }

    void Update()
    {
        CastRays();
    }

    void CastRays()
    {
        Vector3[] directions = new Vector3[numRays];
        float angleStep = 360f / numRays;

        for (int i = 0; i < numRays; i++)
        {
            float angle = i * angleStep;
            directions[i] = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
        }

        foreach (var height in heights)
        {
            Vector3 origin = transform.position + Vector3.up * height;
            foreach (var direction in directions)
            {
                Ray ray = new Ray(origin, direction);
                RaycastHit[] hits = Physics.RaycastAll(ray, detectionRadius, enemyLayer);

                foreach (var hit in hits)
                {
                    Debug.Log("Hai colpito qualcosa");
                    Rigidbody hitRb = hit.collider.attachedRigidbody;
                    if (hitRb != null && !hitRb.isKinematic && hitRb.GetComponent<Enemy>() != null)
                    {
                        Debug.Log("Nemico intersecato");
                        Vector3 pushDirection = hit.point - origin;
                        pushDirection.y = 0; // Non spingere verticalmente
                        pushDirection.Normalize();

                        hitRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                    }
                }

                // Disegna il raggio per il debug
                Debug.DrawRay(origin, direction * detectionRadius, Color.red);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (rb != null)
        {
            Gizmos.color = Color.red;
            Vector3[] directions = new Vector3[numRays];
            float angleStep = 360f / numRays;

            for (int i = 0; i < numRays; i++)
            {
                float angle = i * angleStep;
                directions[i] = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
            }

            foreach (var height in heights)
            {
                Vector3 origin = transform.position + Vector3.up * height;
                foreach (var direction in directions)
                {
                    Gizmos.DrawRay(origin, direction * detectionRadius);
                }
            }
        }
    }
}
