using UnityEngine;

public class UIremoteController: MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Metodo per settare lo sprite come attivo
    public void SetActive(bool isActive)
    {
        spriteRenderer.enabled = isActive;
    }

    // Metodo per settare lo sprite come passivo
    public void SetPassive(bool isPassive)
    {
        spriteRenderer.enabled = !isPassive;
    }    
}
