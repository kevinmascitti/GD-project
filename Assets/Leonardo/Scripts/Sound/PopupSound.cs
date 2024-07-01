using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupSound : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip grabSound;
    [SerializeField] private AudioClip usedSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayerCharacter.OnGrabbed += PlayGrabbedSound;
        PlayerCharacter.OnUsed += PlayUsedSound;
    }

    private void PlayGrabbedSound(object sender, System.EventArgs e)
    {
        audioSource.clip = grabSound;        
        audioSource.Play();
    }

    private void PlayUsedSound(object sender, System.EventArgs e)
    {
        audioSource.clip = usedSound;
        audioSource.Play();
    }

}
