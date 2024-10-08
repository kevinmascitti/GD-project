using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CameraMovement : MonoBehaviour
{
    private Transform target;
    private PlayerCharacter player;
    private GameObject depthPoint;
    private bool isLevelInitialized = false;
    [SerializeField] private float cameraMovementSpeed;
    [SerializeField] private int distanceFromLevelLimit;
    [SerializeField] private int playerDistanceFromCamera;

    public void Awake()
    {
        target = GameObject.Find("Player").transform;
        player = target.GetComponent<PlayerCharacter>();
        depthPoint = GameObject.Find("DepthPoint");

        LevelManager.OnInitializedLevels += StartUpdate;
        PlayerCharacter.OnGameOver += StopUpdate;
    }

    private void OnDestroy()
    {
        LevelManager.OnInitializedLevels -= StartUpdate;
        PlayerCharacter.OnGameOver -= StopUpdate;
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraMovementSpeed);
    }

    private void Update()
    {
        if (isLevelInitialized && player.currentRoom)
        {
            float distanceMin = Math.Abs(target.transform.position.x - player.currentRoom.enterWall.transform.position.x);
            float distanceMax = Math.Abs(target.transform.position.x - player.currentRoom.exitWall.transform.position.x);
            if (distanceMin > distanceFromLevelLimit && distanceMax > distanceFromLevelLimit)
                FollowTarget();
            Vector3 depthPointPos = transform.position;
            depthPointPos.z += playerDistanceFromCamera;
            depthPoint.transform.position = depthPointPos;
        }
    }

    private void StartUpdate(object sender, EventArgs args)
    {
        isLevelInitialized = true;
    }
    
    private void StopUpdate(object sender, EventArgs args)
    {
        isLevelInitialized = false;
    }
}
