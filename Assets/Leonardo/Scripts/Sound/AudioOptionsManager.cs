using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioOptionsManager : MonoBehaviour
{
    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string GeneralPref = "GeneralPref";
    private static readonly string SoundEffectsPref = "SoundEffectsPref";
    private static readonly string MusicPref = "MusicPref";
    private int firstPlayInt;
    private float generalFloat, soundEffectsFloat, musicFloat;
    public Slider generalSlider, soundEffectsSlider, musicSlider;
    // public AudioMixer generalAudio;
    public AudioMixer mixer;

    void Start()
    {
        firstPlayInt = PlayerPrefs.GetInt(FirstPlay);

        if (firstPlayInt == 0) {
            Debug.Log("First play");
            generalFloat = 0.5f;
            soundEffectsFloat = 0.75f;
            musicFloat = 0.75f;
            
            generalSlider.value = generalFloat;
            soundEffectsSlider.value = soundEffectsFloat;
            musicSlider.value = musicFloat;

            PlayerPrefs.SetFloat(GeneralPref, generalFloat);
            PlayerPrefs.SetFloat(SoundEffectsPref, soundEffectsFloat);
            PlayerPrefs.SetFloat(MusicPref, musicFloat);
            PlayerPrefs.SetInt(FirstPlay, -1);
        }
        else {
            Debug.Log("Not first play");

            generalFloat = PlayerPrefs.GetFloat(GeneralPref);
            generalSlider.value = generalFloat;

            soundEffectsFloat = PlayerPrefs.GetFloat(SoundEffectsPref);
            soundEffectsSlider.value = soundEffectsFloat;

            musicFloat = PlayerPrefs.GetFloat(MusicPref);
            musicSlider.value = musicFloat;
        }
    }

    public void SaveSoundSettings() {
        PlayerPrefs.SetFloat(GeneralPref, generalSlider.value);
        PlayerPrefs.SetFloat(SoundEffectsPref, soundEffectsSlider.value);
        PlayerPrefs.SetFloat(MusicPref, musicSlider.value);
    }

    void OnApplicationFocus(bool inFocus) {
        if (!inFocus) {
            SaveSoundSettings();
        }
    }

    public void UpdateSound() {
        if (generalSlider.value > 0) {
            mixer.SetFloat("MasterVolume", Mathf.Log10(generalSlider.value) * 20);
        }
        else {
            mixer.SetFloat("MasterVolume", -80);
        }
        
        if (soundEffectsSlider.value > 0) {
            mixer.SetFloat("SFXVolume", Mathf.Log10(soundEffectsSlider.value) * 20);
        }
        else {
            mixer.SetFloat("SFXVolume", -80);
        }

        if (musicSlider.value > 0) {
            mixer.SetFloat("MusicVolume", Mathf.Log10(musicSlider.value) * 20);
        }
        else {
            mixer.SetFloat("MusicVolume", -80);
        }
    }
}
