using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    protected bool activated = false;

    [SerializeField] protected GameObject icon;
    private SmokeCell linkedCell;

    protected virtual void Awake()
    {
        SetIcons(false);
    }

    protected virtual void Update()
    {
        if (linkedCell == null || !linkedCell.isVisible)
            SetIcons(true);

    }

    public void AssignSmokeCell(SmokeCell linkedCell)
    {
        this.linkedCell = linkedCell;
        SetIcons(false);
    }

    protected virtual void SetIcons(bool state)
    {
        if (icon)
            icon.SetActive(state);
    }

    public virtual bool GetStatus()
    {
        return activated;
    }
}
