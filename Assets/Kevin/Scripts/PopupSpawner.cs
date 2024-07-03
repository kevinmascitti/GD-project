using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PopupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private ComboCounter comboCounter;
    [SerializeField] private int popupSpeed = 10;
    [SerializeField] private List<int> thresholdList = new List<int>();
    [SerializeField] private List<string> popupNamesList = new List<string>();
    private List<Tuple<int, string>> popupList = new List<Tuple<int, string>>();
    private Dictionary<int, bool> isPopupSpawned = new Dictionary<int, bool>();
    private int lastCounter;
    private Vector3 spawningDirection = Vector3.forward;
    private Quaternion spawningRotation = Quaternion.Euler(0, 0, 0);

    [Header("Sound Settings")]
    [SerializeField] private AudioClip[] spawnAudioClips;
    private AudioSource audioSource;

    void Start()
    {
        for (int i = 0; i < thresholdList.Count; i++)
        {
            popupList.Add(new Tuple<int, string>(thresholdList[i], popupNamesList[i]));
            isPopupSpawned.Add(thresholdList[i], false);
        }

        ComboCounter.OnCounterIncreased += CheckCounter;
        ComboCounter.OnCounterInitialized += InitializePopup;
        audioSource = GetComponent<AudioSource>();
    }

    private void CheckCounter(object sender, int args)
    {
        foreach (Tuple<int, string> tuple in popupList)
        {
            if (comboCounter.counter >= tuple.Item1 && lastCounter < tuple.Item1 && !isPopupSpawned[tuple.Item1])
            {
                SpawnPopup(tuple.Item2);
                lastCounter = comboCounter.counter;
                isPopupSpawned[tuple.Item1] = true;
                break;
            }
        }
    }

    private void InitializePopup(object sender, EventArgs args)
    {
        lastCounter = 0;
        for(int i = 0; i < isPopupSpawned.Count; i++)
        {
            isPopupSpawned[i] = false;
        }
    }

    private void SpawnPopup(string comboName)
    {
        audioSource.clip = spawnAudioClips[UnityEngine.Random.Range(0, spawnAudioClips.Length)];
        audioSource.Play();
        Vector3 spawningPosition = player.transform.position;
        GameObject popup = Instantiate(Resources.Load("Popup/" + comboName), spawningPosition, spawningRotation).GameObject().transform.GetChild(0).gameObject;
        popup.GetComponent<Rigidbody>().AddForce(spawningDirection * popupSpeed, ForceMode.Impulse);
        popup.GetComponent<Grabbable>().room = player.GetComponent<PlayerCharacter>().currentRoom;
    }
    
}
