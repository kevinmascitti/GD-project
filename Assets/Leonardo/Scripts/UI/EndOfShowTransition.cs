using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class EndOfShowTransition : MonoBehaviour
{
    [SerializeField] private RawImage EndOfShowImage;
    [SerializeField] private VideoPlayer EndOfShowVideo;
    [Range(0.25f, 3f)] public float transitionTime = 2f;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip EndOfShowClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Room.OnLevelCompleted += StartTransition;
    }

    public void StartTransition(object sender, RoomManager args)
    {
        // Debug.Log("EndOfShow StartTransition");
        StartCoroutine(Transition());
    }

    public IEnumerator Transition()
    {
        // Debug.Log("EndOfShow Transition");
        musicSource.Stop();
        audioSource.clip = EndOfShowClip;
        audioSource.Play();
        EndOfShowImage.gameObject.SetActive(true);
        EndOfShowVideo.gameObject.SetActive(true);
        EndOfShowVideo.Play();
        yield return new WaitForSeconds(transitionTime);
        EndOfShowVideo.Stop();
        EndOfShowVideo.gameObject.SetActive(true);
        EndOfShowImage.gameObject.SetActive(false);
        audioSource.Stop(); 
        musicSource.Play();
    }
    public void OnDestroy()
    {
        Room.OnLevelCompleted -= StartTransition;
    }
}
