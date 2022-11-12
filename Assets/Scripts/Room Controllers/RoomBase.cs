using System.Collections.Generic;
using UnityEngine;

public class RoomBase : MonoBehaviour
{
    protected bool playerEntered = false;
    protected bool playerInRoom = false;
    [HideInInspector] public bool clearedRoom = false;

    [Header("Setup")]
    public Vector2 roomDimensions = new Vector2(4, 4);
    protected Vector2 roomMin;
    protected Vector2 roomMax;
    protected SmokeCloud linkedSmoke;

    [HideInInspector] public List<DoorBase> linkedDoors = new List<DoorBase>();

    protected virtual void OnDrawGizmos()
    {
        CalcRoomDimensions();

        Gizmos.color = Color.grey;
        Gizmos.DrawWireCube(new Vector2(roomMax.x + roomMin.x, roomMax.y + roomMin.y) / 2,
                            new Vector2(roomMax.x - roomMin.x, roomMax.y - roomMin.y));
    }

    protected virtual void Awake()
    {
        CalcRoomDimensions();
        FindDoors();
    }

    protected void CalcRoomDimensions()
    {
        roomMin = (Vector2)transform.position - roomDimensions / 2;
        roomMax = (Vector2)transform.position + roomDimensions / 2;
    }

    protected virtual void FindDoors()
    {
        Collider2D[] objs = Physics2D.OverlapAreaAll(roomMin, roomMax, 1 << GameController.INTERACTIVE_LAYER);
        foreach (Collider2D o in objs)
        {
            DoorBase door = o.GetComponent<DoorBase>();
            if (door)
            {
                door.RegisterRoom(this);
                linkedDoors.Add(door);
            }
        }            
    }

    public virtual void ConnectDoor(DoorBase targetDoor)
    {
        linkedDoors.Add(targetDoor);
    }

    public void AssignSmokeCloud(SmokeCloud smoke)
    {
        linkedSmoke = smoke;
    }

    #region Getters/Setters
    public bool PlayerInRoom()
    {
        return playerInRoom;
    }

    public Vector2 GetRoomMin()
    {
        return roomMin;
    }

    public Vector2 GetRoomMax()
    {
        return roomMax;
    }
    #endregion

    #region Player Movement callbacks
    public virtual void OnPlayerEntering(DoorBase door)
    {
        if (!playerEntered)
        {
            SmokeScreen.instance.RevealSmokeCloud(linkedSmoke);
        }
    }

    public virtual void OnPlayerEntered(DoorBase door)
    {
        if (!playerEntered)
        {
            playerEntered = true;
            OnPlayerFirstEntered(door);
        }

        playerInRoom = true;
    }

    protected virtual void OnPlayerFirstEntered(DoorBase door)
    {
        clearedRoom = true;
    }

    public virtual void OnPlayerExiting(DoorBase door)
    {
        playerInRoom = false;
    }

    public virtual void OnPlayerExited(DoorBase door)
    {

    }
    #endregion
}