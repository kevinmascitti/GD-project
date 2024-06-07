using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICounterHit : MonoBehaviour
{
    [NonSerialized] public bool isCounterVisible;
    private bool isFading;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float rotationDuration = 0.2f;
    [SerializeField] private GameObject hitsWord;

    void Start()
    {
        hitsWord = GameObject.Find("HitsWord");
        ComboCounter.OnCounterIncreased += ShowCounterHit;
        ComboCounter.OnCounterInitialized += HideCounterHit;
    }

    private void ShowCounterHit(object sender, int args)
    {
        if (args > 1)
        {
            isFading = false;
            GetComponent<TMP_Text>().text = args.ToString();
            StartCoroutine(SpinAround());

            if (!isCounterVisible)
            {
                isCounterVisible = true;
                Color color = GetComponent<TMP_Text>().color;
                Color color2 = hitsWord.GetComponent<TMP_Text>().color;
                
                GetComponent<TMP_Text>().color = new Color(color.r, color.g, color.b, 1f);
                hitsWord.GetComponent<TMP_Text>().color = new Color(color2.r, color2.g, color2.b, 1f);
                
                // GetComponent<TMP_Text>().gameObject.SetActive(true);
                // GetComponentInChildren<TMP_Text>().gameObject.SetActive(true);
            }
        }
    }
    
    private void HideCounterHit(object sender, EventArgs args)
    {
        if (isCounterVisible)
        {
            isCounterVisible = false;
            StartCoroutine(StartFadingOut());
            // GetComponent<TMP_Text>().gameObject.SetActive(false);
            // GetComponentInChildren<TMP_Text>().gameObject.SetActive(false);
        }
    }

    IEnumerator SpinAround()
    {
        float elapsedTime = 0f;
        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float rotationAngle = Mathf.Lerp(0f, 360f, elapsedTime / rotationDuration);
            GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, rotationAngle);
            yield return null;
        }
        GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0f);
    }

    IEnumerator StartFadingOut()
    {
        isFading = true;
        Color color = GetComponent<TMP_Text>().color;
        Color color2 = hitsWord.GetComponent<TMP_Text>().color;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration && isFading)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            if (isFading)
            {
                GetComponent<TMP_Text>().color = new Color(color.r, color.g, color.b, alpha);
                hitsWord.GetComponent<TMP_Text>().color = new Color(color2.r, color2.g, color2.b, alpha);
            }

            yield return null;
        }

        if (isFading)
        {
            GetComponent<TMP_Text>().color = new Color(color.r, color.g, color.b, 0f);
            hitsWord.GetComponent<TMP_Text>().color = new Color(color2.r, color2.g, color2.b, 0f);

            // GetComponent<TMP_Text>().gameObject.SetActive(false);
            // GetComponentInChildren<TMP_Text>().gameObject.SetActive(false);
            isFading = false;
        }
    }

}
