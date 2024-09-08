using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroductionVideoSequence : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    private bool videoIsReady = false;

    void Start()
    {
        StartCoroutine(Timer());
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(videoIsReady && !videoPlayer.isPlaying)
        {
            Debug.Log("INTRO SCENE - Video is done playing");
            Cursor.visible = true;
            SceneManager.LoadScene(1);
        }
        if (Input.anyKey)
        {
            videoPlayer.Stop();
            Debug.Log("INTRO SCENE - Video is beeing skipped");
            Cursor.visible = true;
            SceneManager.LoadScene(1);
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(3);
        videoIsReady = true;
    }
}
