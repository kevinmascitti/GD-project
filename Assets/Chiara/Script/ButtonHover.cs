using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;


public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 hoverScale = new Vector3(1.05f, 1.05f, 1.05f); // Scale to increase to
    public float rotationAmount = 5f; // Rotation amount for the wobble effect
    public float rotationSpeed = 5f; // Speed of the rotation
    public float transitionDuration = 0.2f; // Duration for scaling transition

    public Sprite selectedImage;
    public Sprite unselectedImage;


    private Vector3 originalScale;
    private Quaternion originalRotation;
    private bool isHovered = false;
    private Coroutine transitionCoroutine;



    void Start()
    {
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
        changeToUnselected();

        GetComponent<Button>().onClick.AddListener(delegate { backToNormal(); });

    }

    void Update()
    {
        if (isHovered)
        {
            // Apply rotation effect
            float rotation = Mathf.Sin(Time.time * rotationSpeed) * rotationAmount;
            transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(ScaleTo(hoverScale, transitionDuration));
        changeToSelected();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(ScaleTo(originalScale, transitionDuration));
        StartCoroutine(RotateTo(originalRotation, transitionDuration));
        changeToUnselected();
    }

    private void backToNormal()
    {
        isHovered = false;
        transform.localScale = originalScale;
        transform.rotation = Quaternion.identity;
        changeToUnselected();
    }

    private void changeToSelected()
    {
        var image = GetComponent<Image>();
        image.sprite = selectedImage;
    }

    private void changeToUnselected()
    {
        var image = GetComponent<Image>();
        image.sprite = unselectedImage;
    }

    private IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        Vector3 initialScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private IEnumerator RotateTo(Quaternion targetRotation, float duration)
    {
        Quaternion initialRotation = transform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}
