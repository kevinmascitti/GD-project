using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ChannelTransition : MonoBehaviour
{
    [SerializeField] private RawImage channelTransitionImage;
    [SerializeField] private VideoPlayer channelTransitionVideo;
    [Range(0.25f, 1.5f)] public float transitionTime = 1.0f;

    [SerializeField] private AudioSource musicSource;
    private AudioSource audioSource;

    void Start()
    {
        PlayerCharacter.OnStartLevel += StartTransition;
        audioSource = GetComponent<AudioSource>();
    }

    public void StartTransition(object sender, EventArgs args)
    {
        StartCoroutine(Transition());
    }

    public IEnumerator Transition()
    {
        musicSource.Stop();
        audioSource.Play();
        channelTransitionVideo.Play();
        channelTransitionImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(transitionTime);
        channelTransitionImage.gameObject.SetActive(false);
        channelTransitionVideo.Stop();
        audioSource.Stop(); 
        musicSource.Play();
    }
    public void OnDestroy()
    {
        PlayerCharacter.OnStartLevel -= StartTransition;
    }
}
