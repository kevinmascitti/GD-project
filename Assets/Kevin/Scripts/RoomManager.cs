using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> rooms = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        rooms[0].GetComponent<Room>().enterWall.gameObject.layer = 0;
        rooms[rooms.Count-1].GetComponent<Room>().exitWall.gameObject.layer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
