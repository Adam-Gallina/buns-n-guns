using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGeneration : MonoBehaviour
{
    public static RoomGeneration Instance;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap wallMap;
    [SerializeField] private Tilemap backgroundMap;

    [Header("Tiles")]
    [SerializeField] private Tile wallTile;
    [SerializeField] private Tile groundTile;

    [Header("Room Controllers")]
    [SerializeField] private GameObject spawnRoomPrefab;

    [Header("Prefabs")]
    [SerializeField] private GameObject doorPrefab;


    private List<Vector2Int> availableWalls = new List<Vector2Int>();

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateRooms()
    {
        CreateSpawnRoom(new Vector2Int(16, 12));
    }

    private void CreateSpawnRoom(Vector2Int size)
    {
        AddRoom(-size / 2, size / 2);
        AddDoor(new Vector2Int(-1, size.y / 2 - 1), new Vector2Int(1, size.y / 2 - 1));

        GameObject spawnRoom = Instantiate(spawnRoomPrefab, transform);
        spawnRoom.GetComponent<SpawnRoom>().roomDimensions = size;
    }

    private void AddRoom(Vector2Int minPos, Vector2Int maxPos)
    {
        for (int x = minPos.x; x < maxPos.x; x++)
        {
            for (int y = minPos.y; y < maxPos.y; y++)
            {
                if (x == minPos.x || x == maxPos.x - 1 ||
                    y == minPos.y || y == maxPos.y - 1)
                {
                    wallMap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
                else
                {
                    backgroundMap.SetTile(new Vector3Int(x, y, 0), groundTile);
                }
            }
        }
    }

    private void AddDoor(Vector2Int p1, Vector2Int p2)
    {
        for (int x = p1.x; x < p2.x; x++)
        {
            for (int y = p1.y; y < p2.y; y++)
            {
                wallMap.SetTile(new Vector3Int(x, y, 0), null);
                backgroundMap.SetTile(new Vector3Int(x, y, 0), groundTile);
            }
        }

        GameObject newDoor = Instantiate(doorPrefab, transform);
        newDoor.transform.position = ((Vector3)(Vector2)(p1 + p2)) / 2;
    }
}
