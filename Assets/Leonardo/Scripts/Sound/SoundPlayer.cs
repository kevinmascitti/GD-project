using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class SoundPlayer
{
    public static AudioSource source;
    public static AudioMixerGroup sfxMixerGroup;
    public static void PlaySound(AudioClip clip, float volume = 1f)
    {
        if(source == null)
        {
            source = new GameObject().AddComponent<AudioSource>();
            source.outputAudioMixerGroup = sfxMixerGroup;
        }
        source.clip = clip;
        source.volume = volume;
        source.Play();

    }
}
