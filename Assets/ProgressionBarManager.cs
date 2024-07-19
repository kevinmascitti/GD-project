using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionBarManager : MonoBehaviour
{
    public Vector3 spawnPoint;
    public Vector3 exitPoint;

    public Vector3 playerPosition;

    public GameObject player;
    
    [SerializeField] private Image fill;
    public float currlvlprogression;
    // Start is called before the first frame update
    void Awake()
    {
        PlayerCharacter.OnStartLevel += UpdatePoints;
    }

    public void UpdatePoints(object sender,RoomManagerArgs args)
    {
        if (args != null && args.level)
        {
            Debug.Log("i valori di spawn point ed exit point sono"+spawnPoint+exitPoint);
            spawnPoint = args.level.inizio.position;
            exitPoint = args.level.fine.position;
            fill.fillAmount = 0.0f;
        }
    }
    // Update is called once per frame
    void Update()
    {
        playerPosition = player.transform.position;
        currlvlprogression =1-(Mathf.Abs( exitPoint.x - playerPosition.x )/ Mathf.Abs(exitPoint.x - spawnPoint.x));
        fill.fillAmount = currlvlprogression;
        Debug.Log(currlvlprogression);
    }
    void OnDestroy()
    {
        PlayerCharacter.OnStartLevel -= UpdatePoints;
    }

}
