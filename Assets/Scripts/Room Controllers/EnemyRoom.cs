using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnFormation { Random, Square, Circle, Oval };
public class EnemyRoom : RoomBase
{
    [Header("Spawning")]
    public bool DEBUG_spawnEnemies = true;
    [SerializeField] protected Wave[] roomWaves;
    [SerializeField] protected Vector2 spawnAreaOffset;
    [SerializeField] protected Vector2 spawnAreaDimensions;
    protected Vector2 minSpawn;
    protected Vector2 maxSpawn;
    public float spawnDistFromPlayer = 3;
    public float spawnDelay = 0.5f;
    protected int currWave = 0;
    protected bool completedWave = false;
    protected bool spawningWaves;

    protected List<GameObject> activeEnemies = new List<GameObject>();

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + spawnAreaOffset, spawnAreaDimensions);
    }

    protected override void Awake()
    {
        base.Awake();

        minSpawn = (Vector2)transform.position + spawnAreaOffset - spawnAreaDimensions / 2;
        maxSpawn = (Vector2)transform.position + spawnAreaOffset + spawnAreaDimensions / 2;
    }
    
    protected void StartSpawningWaves()
    {
        foreach (DoorBase door in linkedDoors)
        {
            door.SetForcedLockState(true);
        }

        StartCoroutine(SpawnWaves());
    }

    protected virtual IEnumerator SpawnWaves()
    {
        spawningWaves = true;

        while (currWave < roomWaves.Length)
        {
            yield return new WaitForSeconds(spawnDelay);
            SpawnNextWave(currWave++);
            yield return new WaitUntil(() => activeEnemies.Count == 0);
        }

        spawningWaves = false;
        OnWavesCleared();
    }

    protected virtual void SpawnNextWave(int wave)
    {
        switch (roomWaves[wave].formation) {
            case SpawnFormation.Random:
                SpawnEnemiesRandom(roomWaves[wave]);
                break;
            case SpawnFormation.Square:
                break;
            case SpawnFormation.Circle:
                SpawnEnemiesCircle(roomWaves[wave]);
                break;
            case SpawnFormation.Oval:
                break;
            default:
                Debug.LogWarning("Unrecognized SpawnFormation: " + roomWaves[currWave].formation + ", defaulting to Random");
                roomWaves[wave].formation = SpawnFormation.Random;
                SpawnNextWave(wave);
                break;
        }
    }

    protected override void OnPlayerFirstEntered(DoorBase door)
    {
        if (DEBUG_spawnEnemies && GameController.instance.DEBUG_spawnEnemies)
            StartSpawningWaves();
    }

    public virtual void OnEnemyDeath(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public virtual void OnWavesCleared()
    {
        OnRoomCleared();

        FloatingText.instance.CreateText(LevelController.instance.player.position, "Room Completed!");
    }

    protected virtual void OnRoomCleared()
    {
        clearedRoom = true;

        foreach (DoorBase door in linkedDoors)
        {
            door.SetForcedLockState(false);
        }
    }

    #region Spawn Formations
    public virtual Vector2 GetRandomSpawnLocation(int tries = 5)
    {
        Vector2 candidateLocation = new Vector2(Random.Range(minSpawn.x, maxSpawn.x),
                                                Random.Range(minSpawn.y, maxSpawn.y));

        if (tries > 0)
        {
            if (Physics2D.OverlapCircle(candidateLocation, spawnDistFromPlayer, 1 << GameController.PLAYER_LAYER))
            {
                return GetRandomSpawnLocation(--tries);
            }
        }

        return candidateLocation;
    }

    protected virtual void SpawnEnemiesRandom(Wave wave)
    {
        foreach (EnemySpawn enemy in wave.enemies)
        {
            activeEnemies.Add(enemy.GetEnemy(this, GetRandomSpawnLocation()));
        }
    }

    protected virtual void SpawnEnemiesCircle(Wave wave)
    {
        float radius = Mathf.Min(spawnAreaDimensions.x, spawnAreaDimensions.y) / 2;
        float deltaAng = 2 * Mathf.PI / wave.enemies.Length;
        for (int i = 0; i < wave.enemies.Length; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + spawnAreaOffset + new Vector2(radius * Mathf.Cos(deltaAng * i), radius * Mathf.Sin(deltaAng * i));
            activeEnemies.Add(wave.enemies[i].GetEnemy(this, spawnPos));
        }
    }
    #endregion
}

[System.Serializable]
public class Wave
{
    public SpawnFormation formation = SpawnFormation.Random;
    public EnemySpawn[] enemies;
}

[System.Serializable]
public class EnemySpawn
{
    public GameObject enemyPrefab;
    public GameObject enemyGun;

    public GameObject GetEnemy(EnemyRoom spawnSource, Vector2 spawnPos)
    {
        GameObject newEnemy = Object.Instantiate(enemyPrefab);
        newEnemy.transform.position = spawnPos;

        if (enemyGun)
            newEnemy.GetComponent<EnemyBase>().AddWeapon(enemyGun);
        newEnemy.GetComponent<EnemyBase>().InitializeEnemy(spawnSource);

        return newEnemy;
    }
}

