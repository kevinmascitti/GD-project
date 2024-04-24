using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Transform target;
    private PlayerCharacter player;
    [SerializeField] private float cameraMovementSpeed;
    [SerializeField] private float distanceFromEndLevel;

    public void Start()
    {
        target = GameObject.Find("Player").transform;
        player = target.GetComponent<PlayerCharacter>();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraMovementSpeed);
    }

    private void Update()
    {
        if(Vector3.Distance(target.transform.position, player.currentRoom.plane.GetComponent<MeshCollider>().bounds.min) < distanceFromEndLevel 
           && Vector3.Distance(target.transform.position, player.currentRoom.plane.GetComponent<MeshCollider>().bounds.max) < distanceFromEndLevel)
            FollowTarget();
    }
}
