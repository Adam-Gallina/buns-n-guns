using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbushRoom : EnemyRoom
{
    [SerializeField] protected float distanceFromStartDoor = 4;
    protected Transform entryDoor;

    protected void Update()
    {
        if (entryDoor && Vector2.Distance(LevelController.instance.player.position, entryDoor.position) > distanceFromStartDoor)
        {
            if (DEBUG_spawnEnemies && GameController.instance.DEBUG_spawnEnemies)
            {
                StartSpawningWaves();
                entryDoor = null;
            }
        }
    }

    protected override void OnPlayerFirstEntered(DoorBase door)
    {
        entryDoor = door.transform;
    }
}
