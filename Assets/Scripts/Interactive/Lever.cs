using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : DirectInteraction
{
    protected override bool DoInteraction(Transform source)
    {
        activated = !activated;
        Debug.Log(name + " is " + activated);
        return true;
    }
}
