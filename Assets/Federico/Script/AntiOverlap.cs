using UnityEngine;
using System.Collections;

public class AntiOverlap : MonoBehaviour
{
    public float detectionRadius = 1.0f; // Raggio per i raggi
    public int numRays = 8; // Numero di raggi da lanciare per ogni livello
    public float pushForce = 10f; // Forza con cui spingere indietro i nemici
    public float[] heights = new float[] { 1.0f }; // Altezze da cui lanciare i raggi
    public LayerMask enemyLayer; // Layer dei nemici
    public float fadeDuration = 0.5f; // Durata del fading
    public float blinkDuration = 1.5f; // Durata del lampeggiamento

    private Rigidbody rb;
    private Renderer playerRenderer;
    private Material originalMaterial;
    private bool isBlinking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Il componente Rigidbody non è presente sul giocatore.");
        }
        else if (!rb.isKinematic)
        {
            Debug.LogWarning("Il Rigidbody del giocatore non è kinematic. Assicurati di impostarlo come kinematic per questo script.");
        }

        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null)
        {
            originalMaterial = playerRenderer.material;
            Debug.Log("Materiale originale: " + originalMaterial.name);
        }
        else
        {
            Debug.LogError("Il componente Renderer non è presente sul giocatore.");
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

                        // Applica la forza al nemico
                        //hitRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                        // Controlla se la nuova posizione è libera
                        /*
                        float distance = Mathf.Clamp(selfPushDistance, minPushDistance, maxPushDistance);
                           Vector3 selfPushDirection = -pushDirection;
                           Vector3 newPosition = transform.position + selfPushDirection * distance;
                           
                        if (!Physics.CheckSphere(newPosition, detectionRadius, enemyLayer))
                        {
                            rb.MovePosition(newPosition);
                        }
                        */
                        // Attiva l'effetto di lampeggiamento
                        
                        if (!isBlinking)
                        {
                            StartCoroutine(BlinkEffect());
                        }
                        
                    }
                }

                // Disegna il raggio per il debug
                Debug.DrawRay(origin, direction * detectionRadius, Color.red);
            }
        }
    }

    IEnumerator BlinkEffect()
    {
        isBlinking = true;
        Color originalColor = originalMaterial.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        float timer = 0f;

        while (timer < blinkDuration)
        {
            Color lerpedColor = Color.Lerp(originalColor, transparentColor, Mathf.PingPong(timer * 2 / fadeDuration, 1));
            playerRenderer.material.color = lerpedColor;
            Debug.Log("Colore attuale: " + lerpedColor);
            timer += Time.deltaTime;
            yield return null;
        }

        playerRenderer.material.color = transparentColor;
        Debug.Log("Ripristinato colore originale: " + originalColor);
        isBlinking = false;
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
