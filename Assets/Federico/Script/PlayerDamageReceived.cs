using System;
using System.Collections;
using UnityEngine;

public class PlayerDamageReceived : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] private float totalFlickerDuration = 2.0f; // Durata totale del lampeggio in secondi
    [SerializeField] private float flickerInterval = 0.1f; // Intervallo di tempo per alternare visibilità (in secondi)
    private SkinnedMeshRenderer playerRenderer;
    private bool isFlickering = false;
    public static event EventHandler OnDamageReceived;

    void Awake()
    {
        if (player != null)
        {
            playerRenderer = player.GetComponentInChildren<SkinnedMeshRenderer>();
        }

        OnDamageReceived += DamageManager;
    }

    void OnDestroy()
    {
        OnDamageReceived -= DamageManager;
    }

    void DamageManager(object sender, EventArgs e)
    {
        if (!isFlickering)
        {
           Debug.Log("coroutine chiamata");
            StartCoroutine(DisappearAndReappear());
        }
        else
        {
            Debug.Log("l'evento è stato chiamato ma non ho fatto nulla perchè sta gia lampeggiando");
        }
    }

    IEnumerator DisappearAndReappear()
    {
        isFlickering = true;
        float elapsedTime = 0f;
        bool visible = true;

        while (elapsedTime < totalFlickerDuration)
        {
            // Alterna la visibilità
            visible = !visible;
            if (playerRenderer != null)
            {
                playerRenderer.enabled = visible;
            }

            // Attendi la durata di ogni flicker
            yield return new WaitForSeconds(flickerInterval);
            elapsedTime += flickerInterval;
        }

        // Assicurati che il player sia visibile alla fine
        if (playerRenderer != null)
        {
            playerRenderer.enabled = true;
        }
        isFlickering = false;
    }

    public static void TriggerDamageReceived()
    {
        OnDamageReceived?.Invoke(null, EventArgs.Empty);
    }
}