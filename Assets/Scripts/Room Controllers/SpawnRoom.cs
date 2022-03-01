using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoom : RoomBase
{
    public bool DEBUG_useExistingPlayer = false;
    [SerializeField] protected Vector2 playerSpawnPos;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + playerSpawnPos, 1.5f);
    }

    public GameObject SpawnPlayer(GameObject playerPrefab, GameObject playerGun)
    {
        if (DEBUG_useExistingPlayer)
            return GameObject.Find("Player");

        GameObject player = Instantiate(playerPrefab, (Vector2)transform.position + playerSpawnPos, Quaternion.identity);
        player.GetComponent<PlayerController>().AddWeapon(playerGun);
        
        OnPlayerEntering(null);
        OnPlayerEntered(null);

        return player;
    }
}
