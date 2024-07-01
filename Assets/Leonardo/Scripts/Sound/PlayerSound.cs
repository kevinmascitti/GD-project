using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSound : MonoBehaviour
{
    [Header("Player hit Sounds")]
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private AudioSource hitAudioSource;
    // [Header(" ")]

    // Start is called before the first frame update
    void Start()
    {
        PlayerDamageReceived.OnDamageReceived += PlayHitSound;
    }

    public void PlayHitSound(object sender, System.EventArgs e)
    {
        hitAudioSource.clip = hitSounds[Random.Range(0, hitSounds.Length)];
        if (!hitAudioSource.isPlaying)
            hitAudioSource.Play();
    }

    void OnDestroy()
    {
        PlayerDamageReceived.OnDamageReceived -= PlayHitSound;
    }
}
