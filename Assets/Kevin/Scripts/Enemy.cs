using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public Room currentRoom;
    public static EventHandler<Room> OnEnemyDeath;

    public void Awake()
    {
        currentRoom = GameObject.Find("Player").GetComponent<PlayerCharacter>().currentRoom;
        if (currentRoom == null)
            currentRoom = transform.parent.GetComponent<Room>();

        LevelManager.OnEndRoom += DestroyGameObject;
        Room.OnEndRoom += DestroyGameObject;
    }

    public void OnDestroy()
    { 
        LevelManager.OnEndRoom -= DestroyGameObject;
        Room.OnEndRoom -= DestroyGameObject;
    }

    public void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        // TO DO animazione e morte
        OnEnemyDeath?.Invoke(this, currentRoom);
        // agigungo la kill al player
        Destroy(gameObject);
    }

    private void DestroyGameObject(object sender, RoomArgs args)
    {
        if (args.room == currentRoom)
        {
            Destroy(gameObject);
        }
    }

}
