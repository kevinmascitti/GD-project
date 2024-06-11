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
    private Quaternion spawningRotation = Quaternion.Euler(270, 0, 0);

    void Start()
    {
        for (int i = 0; i < thresholdList.Count; i++)
        {
            popupList.Add(new Tuple<int, string>(thresholdList[i], popupNamesList[i]));
            isPopupSpawned.Add(thresholdList[i], false);
        }

        ComboCounter.OnCounterIncreased += CheckCounter;
        ComboCounter.OnCounterInitialized += InitializePopup;
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
        Vector3 spawningPosition = player.transform.position;
        Rigidbody rb = Instantiate(Resources.Load(comboName), spawningPosition, spawningRotation).GetComponent<Rigidbody>();
        rb.AddForce(spawningDirection * popupSpeed, ForceMode.Impulse);
    }
    
}
