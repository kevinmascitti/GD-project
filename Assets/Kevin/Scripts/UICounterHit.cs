using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UICounterHit : MonoBehaviour
{
    [NonSerialized] public bool isCounterVisible;
    private bool isFading;
    private int currentUIColor = 0;
    private int currentLerpStep = 0;
    [SerializeField] private Color uiHitsColor;
    [SerializeField] private List<Color> uiColors = new List<Color>();
    [SerializeField] private int stepColorLerp = 10;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float rotationDuration = 0.2f;
    [SerializeField] private GameObject hitsWord;

    void Start()
    {
        hitsWord = GameObject.Find("HitsWord");
        
        uiColors.Add(Color.white);
        uiColors.Add(Color.blue);
        uiColors.Add(Color.green);
        uiColors.Add(Color.yellow);
        uiColors.Add(Color.magenta);
        uiColors.Add(Color.red);
        GetComponent<TMP_Text>().color = uiColors[currentUIColor];
        hitsWord.GetComponent<TMP_Text>().color = uiHitsColor;
        
        GetComponent<TMP_Text>().alpha = 0;
        hitsWord.GetComponent<TMP_Text>().alpha = 0;

        ComboCounter.OnCounterIncreased += ShowCounterHit;
        ComboCounter.OnCounterInitialized += HideCounterHit;
    }

    private void OnDestroy()
    {
        ComboCounter.OnCounterIncreased -= ShowCounterHit;
        ComboCounter.OnCounterInitialized -= HideCounterHit;
    }

    private void ShowCounterHit(object sender, int args)
    {
        if (args > 1)
        {
            isFading = false;
            GetComponent<TMP_Text>().text = args.ToString();
            StartCoroutine(SpinAround());
            ChangeColor();

            if (!isCounterVisible)
            {
                isCounterVisible = true;
                Color color = GetComponent<TMP_Text>().color;
                Color color2 = hitsWord.GetComponent<TMP_Text>().color;
                
                GetComponent<TMP_Text>().color = new Color(color.r, color.g, color.b, 1f);
                hitsWord.GetComponent<TMP_Text>().color = new Color(color2.r, color2.g, color2.b, 1f);
                
            }
        }
    }
    
    private void HideCounterHit(object sender, EventArgs args)
    {
        if (isCounterVisible)
        {
            isCounterVisible = false;
            StartCoroutine(StartFadingOut());
            currentLerpStep = 0;
            currentUIColor = 0;
            GetComponent<TMP_Text>().color = uiColors[currentUIColor];
            // GetComponent<TMP_Text>().gameObject.SetActive(false);
            // GetComponentInChildren<TMP_Text>().gameObject.SetActive(false);
        }
    }

    private void ChangeColor()
    {
        currentLerpStep++;
        if (currentLerpStep == stepColorLerp)
        {
            currentLerpStep = 0;
            currentUIColor = (currentUIColor + 1) % uiColors.Count;
        }
        int nextUIColor = (currentUIColor + 1) % uiColors.Count;
        
        Color startColor = uiColors[currentUIColor];
        Color endColor = uiColors[nextUIColor];

        GetComponent<TMP_Text>().color = Color.Lerp(startColor, endColor, (float) currentLerpStep / stepColorLerp);
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

            isFading = false;
        }
    }

}
