using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControllerButton : MonoBehaviour
{
    [SerializeField] private GameObject vfx;
    [SerializeField] private GameObject text;

    [SerializeField] private float buttonMoveDistance = 2f;
    [SerializeField] private float buttonMoveDuration = 2f;
    [SerializeField] private float fadeDuration = 2f;
    
    // Start is called before the first frame update
    void Start()
    {
        Boss.OnBossDeath += AnimateButton;
    }

    private void AnimateButton(object sender, ControllerButton button)
    {
        if (button == this)
        {
            vfx.SetActive(true); // attiva VFX button
            StartCoroutine(MoveButtonUp());
        }
    }
    
    IEnumerator MoveButtonUp()
    {
        float elapsedTime = 0f;
        Vector3 newPosition = transform.position + transform.up * buttonMoveDistance;

        while (elapsedTime < buttonMoveDuration)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, elapsedTime / buttonMoveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = newPosition;
        yield return new WaitForSeconds(1f);
        StartCoroutine(StartShowingText());
        yield return new WaitForSeconds(fadeDuration + 2f);
        vfx.SetActive(false); // disattiva VFX button
        yield return new WaitForSeconds(1f);
        StartCoroutine(StartHiding());
    }

    IEnumerator StartShowingText()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(1);
    }

    IEnumerator StartHiding()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    
    private void SetAlpha(float alpha)
    {
        Color color = text.GetComponent<TMP_Text>().color;
        color.a = alpha;
        text.GetComponent<TMP_Text>().color = color;
    }
    
}
