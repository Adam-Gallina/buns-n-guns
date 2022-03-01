using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DirectInteraction : Interactive
{
    protected bool canInteract = true;

    public virtual void Interact(Transform source) {
        if (canInteract)
            DoInteraction(source);
    }

    protected abstract bool DoInteraction(Transform source);
}
