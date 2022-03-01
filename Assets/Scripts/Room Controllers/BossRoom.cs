using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : EnemyRoom
{
    [Header("Boss")]
    [SerializeField] protected EnemySpawn boss;
    protected GameObject bossInstance;

    [SerializeField] protected Vector2 bossStartPos;
    [SerializeField] protected Vector2 playerStartPos;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + bossStartPos, 1.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + playerStartPos, 1.5f);
    }

    protected virtual void SpawnBoss()
    {
        bossInstance = boss.GetEnemy(this, (Vector2)transform.position + bossStartPos);
    }

    public override void OnEnemyDeath(GameObject enemy)
    {
        if (enemy == bossInstance)
        {
            OnRoomCleared();
        }
        else
        {
            base.OnEnemyDeath(enemy);
        }
    }

    public override void OnPlayerEntering(DoorBase door)
    {
        base.OnPlayerEntering(door);
        CameraController.instance.AnimateZoom(0.75f, 0.5f);
    }

    protected override void OnPlayerFirstEntered(DoorBase door)
    {
        StartCoroutine(MovePlayerToStart());
    }

    public override void OnPlayerExiting(DoorBase door)
    {
        base.OnPlayerExiting(door);

        CameraController.instance.AnimateZoom(1, 0.5f);
    }

    public override void OnWavesCleared()
    {
        if (GameController.instance.DEBUG_spawnBosses)
            SpawnBoss();
        else
            base.OnWavesCleared();
    }

    protected override void OnRoomCleared()
    {
        base.OnRoomCleared();

        LevelController.instance.PlayerWin();
    }

    protected IEnumerator MovePlayerToStart()
    {
        MovementBase player = LevelController.instance.player.GetComponent<MovementBase>();

        player.SetAnimationState(true);
        yield return new WaitUntil(() => player.MoveToTarget((Vector2)transform.position + playerStartPos, minDist: 0.15f));
        player.SetAnimationState(false);

        if (DEBUG_spawnEnemies)
            StartSpawningWaves();

    }
}
