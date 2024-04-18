using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public GameObject spawnPoint;
    public Scene scene;
    public GameObject cameraPosition;
    public bool isLocked = false;
    public Room prevRoom;
    public Room nextRoom;
    public BoxCollider enterWall;
    public BoxCollider exitWall;
}
