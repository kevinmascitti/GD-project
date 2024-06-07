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
    private List<Tuple<int, string>> popupList = new List<Tuple<int, string>>();
    private int lastCounter;
    private Vector3 spawningDirection = Vector3.forward;
    private Quaternion spawningRotation = Quaternion.Euler(270, 0, 0);

    void Start()
    {
        popupList.Add(new Tuple<int, string>(5, "Good"));
        popupList.Add(new Tuple<int, string>(10, "Great"));
        popupList.Add(new Tuple<int, string>(15, "Amazing"));
        popupList.Add(new Tuple<int, string>(25, "Marvelous"));
        popupList.Add(new Tuple<int, string>(35, "Wonderful"));
        popupList.Add(new Tuple<int, string>(45, "Godlike"));
        popupList.Add(new Tuple<int, string>(55, "Unbelievable"));

        ComboCounter.OnCounterIncreased += CheckCounter;
    }

    private void CheckCounter(object sender, int args)
    {
        foreach (Tuple<int, string> tuple in popupList)
        {
            if (comboCounter.counter >= tuple.Item1 && lastCounter < tuple.Item1)
            {
                SpawnPopup(tuple.Item2);
                lastCounter = comboCounter.counter;
            }
            else
            {
                return;
            }
        }
    }

    private void SpawnPopup(string comboName)
    {
        Vector3 spawningPosition = player.transform.position;
        Rigidbody rb = Instantiate(Resources.Load(comboName), spawningPosition, spawningRotation).GetComponent<Rigidbody>();
        rb.AddForce(spawningDirection * popupSpeed, ForceMode.Impulse);
    }
    
}
