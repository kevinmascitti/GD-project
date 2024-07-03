using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ComboCounterSound : MonoBehaviour
{
    private float pitch = 1.0f;
    private AudioSource audioSource;
    [Range(0.001f, 0.05f)] public float pitchIncrement = 0.025f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ComboCounter.OnCounterIncreased += PlaySound;
        ComboCounter.OnCounterInitialized += ResetPitch;    
    }

    private void OnDestroy()
    {
        ComboCounter.OnCounterIncreased -= PlaySound;
        ComboCounter.OnCounterInitialized -= ResetPitch;    
    }

    private void PlaySound(object sender, int counter)
    {
        audioSource.pitch = pitch;
        audioSource.Play();
        pitch += pitchIncrement;
    }

    private void ResetPitch(object sender, System.EventArgs e)
    {
        pitch = 1.0f;
    }


}
