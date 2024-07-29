using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Blink: MonoBehaviour
{
    public float blinkInterval = 0.4f;
    private Image image;
    public bool blinking = false;

    void Start()
    {
        image = GetComponent<Image>();
        StartBlinking();
    }

    public void StartBlinking()
    {
        StartCoroutine(BlinkStart());
        
    }

    private void Update()
    {
        if (!blinking)
        {
            StartBlinking();
            blinking = true;
        }
    }


    IEnumerator BlinkStart()
    {
        while (true)
        {
            image.enabled = !image.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}