using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DoorBase : DirectInteraction
{
    protected List<RoomBase> linkedRooms = new List<RoomBase>();

    protected bool playerOpened = false;
    protected bool forceLocked = false;

    protected BoxCollider2D coll;
    protected Animator anim;

    protected override void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    protected override void SetIcons(bool state)
    {
        if (icon)
            icon.SetActive(!playerOpened && state);
    }

    public virtual void RegisterRoom(RoomBase room)
    {
        linkedRooms.Add(room);
    }

    public void SetForcedLockState(bool lockState)
    {
        forceLocked = lockState;
        anim.SetBool("Force Locked", forceLocked);
    }

    public virtual void Close()
    {
        activated = false;
        coll.isTrigger = false;
        coll.gameObject.tag = GameController.COLLIDABLE_TAG;
        anim.SetBool("Open", false);
    }

    public virtual void Open()
    {
        activated = true;
        coll.isTrigger = true;
        coll.gameObject.tag = GameController.INCORPOREAL_TAG;
        anim.SetBool("Open", true);
    }

    protected override bool DoInteraction(Transform source)
    {
        if (linkedRooms.Count != 2)
        {
            Debug.LogError("Cannot open door, not enough linked rooms (" + linkedRooms.Count + ")");
            return false;
        }

        if (!forceLocked)
        {
            MovementBase target = source.GetComponent<MovementBase>();
            if (target)
            {
                StartCoroutine(MoveThroughDoor(target));
                playerOpened = true;
                return true;
            }
            else
            {
                Debug.LogWarning("Cannot open for " + source.name);
            }
        }

        return false;
    }

    protected IEnumerator MoveThroughDoor(MovementBase target)
    {
        Open();

        Vector2 targetFromOrigin = target.transform.position - transform.position;
        Vector2 reflectedFromOrigin = Vector2.Reflect(targetFromOrigin, transform.up);
        Vector2 reflectedPos = (Vector2)transform.position + reflectedFromOrigin;

        RoomBase nextRoom = linkedRooms[0] == LevelController.instance.currRoom ? linkedRooms[1] : linkedRooms[0];

        if (nextRoom) 
        {
            LevelController.instance.currRoom?.OnPlayerExiting(this);
            nextRoom.OnPlayerEntering(this);

            target.SetAnimationState(true);
            if (Physics2D.Linecast(target.transform.position, reflectedPos, 1 << GameController.WALL_LAYER) ||
                Physics2D.Linecast(target.transform.position, reflectedPos + (Vector2)transform.right * 1, 1 << GameController.WALL_LAYER) ||
                Physics2D.Linecast(target.transform.position, reflectedPos - (Vector2)transform.right * 1, 1 << GameController.WALL_LAYER))
            {
                reflectedPos = (Vector2)transform.position + (reflectedFromOrigin - targetFromOrigin).normalized * 1.5f;
                yield return new WaitUntil(() => target.MoveToTarget(transform.position, minDist: 0.15f));
            }
            yield return new WaitUntil(() => target.MoveToTarget(reflectedPos, minDist: 0.15f));

            target.SetAnimationState(false);

            LevelController.instance.currRoom?.OnPlayerExited(this);
            nextRoom.OnPlayerEntered(this);

            LevelController.instance.UpdateCurrRoom(nextRoom);
        }

        Close();
    }
}
