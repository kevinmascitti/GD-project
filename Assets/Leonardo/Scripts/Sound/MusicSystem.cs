using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSystem : MonoBehaviour
{
    [SerializeField] private AudioClip[] musicClips;
    private PlayerCharacter player;
    private AudioSource audioSource;
    private int currentLevelIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCharacter>();
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayLevelMusic();
    }

    public void PlayLevelMusic()
    {
        if (player.currentLevel != null) {
            if (player.currentLevel.gameObject.name == "Level1" && currentLevelIndex != 0)
            {
                audioSource.clip = musicClips[0];
                currentLevelIndex = 0;
            }
            else if (player.currentLevel.gameObject.name == "Level2" && currentLevelIndex != 1)
            {
                audioSource.clip = musicClips[1];
                currentLevelIndex = 1;
            } 
            else if (player.currentLevel.gameObject.name == "Level3" && currentLevelIndex != 2)
            {
                audioSource.clip = musicClips[2];
                currentLevelIndex = 2;
            }
        }
        else {
            Debug.LogError("Player current level in MusicSystem is null, music cannot be switched");
        }
    }
}

