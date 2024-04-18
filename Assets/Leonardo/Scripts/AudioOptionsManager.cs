using UnityEngine;
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
    public AudioSource generalAudio;
    public AudioSource musicAudio;
    public AudioSource[] soundEffectsAudio;

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
        AudioListener.volume = generalSlider.value;
        musicAudio.volume = musicSlider.value;
        foreach (AudioSource sound in soundEffectsAudio) {
            sound.volume = soundEffectsSlider.value;
        }
    }
}
