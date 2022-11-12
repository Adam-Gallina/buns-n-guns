using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoomGen : LevelController
{

    public override void StartLevel(GameObject playerPrefab, GameObject playerGunPrefab)
    {
        RoomGeneration.Instance.GenerateRooms();

        SpawnPlayer(playerPrefab, playerGunPrefab);
        SetPause(false);

        OnLevelReady();
    }


    protected override IEnumerator PlayStory()
    {
        yield return null;
    }
}
