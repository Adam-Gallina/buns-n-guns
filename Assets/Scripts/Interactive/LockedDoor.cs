using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : DoorBase
{
    [SerializeField] protected bool startLocked = true;
    protected bool locked = true;
    protected List<LockedDoor> linkedDoors = new List<LockedDoor>();
    [SerializeField] protected bool canBeLinkedTo = true;

    [Header("Unlocking requirements")]
    [SerializeField] protected InventoryItem requiredItem;
    [SerializeField] protected bool consumeItemOnUnlock = true;

    [Header("Visuals")]
    [SerializeField] protected GameObject keyhole;
    [SerializeField] protected GameObject lockIcon;

    private void OnValidate()
    {
        if (lockIcon)
            lockIcon.transform.eulerAngles = Vector3.zero;
    }

    protected override void Awake()
    {
        base.Awake();

        if (startLocked)
            LockDoor();
        else
            UnlockDoor();

        if (lockIcon)
            lockIcon.transform.eulerAngles = Vector3.zero;
    }

    protected void Start()
    {
        foreach (LockedDoor l in transform.parent.GetComponentsInChildren<LockedDoor>())
        {
            if (l == this || !l.canBeLinkedTo)
                continue;

            foreach (RoomBase r in linkedRooms)
            {
                foreach (RoomBase r2 in l.linkedRooms)
                {
                    if (r == r2)
                    {
                        linkedDoors.Add(l);
                    }
                }
            }
        }
    }

    protected override void SetIcons(bool state)
    {
        if (icon)
            icon.SetActive(!keyhole.activeSelf && !playerOpened && state);

        if (lockIcon)
            lockIcon.SetActive(keyhole.activeSelf && state);
    }

    public virtual void UnlockDoor()
    {
        locked = false;
        anim.SetTrigger("Unlock");
        keyhole.SetActive(false);
    }

    public virtual void LockDoor()
    {
        locked = true;
        anim.SetTrigger("Lock");
        keyhole.SetActive(true);
    }

    protected override bool DoInteraction(Transform source)
    {
        if (!forceLocked)
        {
            if (!locked)
                return base.DoInteraction(source);

            Inventory inv = source.GetComponent<Inventory>();
            if (inv)
            {
                if (inv.RemoveItem(requiredItem))
                {
                    UnlockDoor();

                    foreach (LockedDoor l in linkedDoors)
                        l.UnlockDoor();

                    FloatingText.instance.CreateText(transform.position, "Unlocked!");
                    return true;
                }
            }
        }

        FloatingText.instance.CreateText(transform.position, "Missing " + requiredItem.itemName + "...");
        return false;
    }
}
