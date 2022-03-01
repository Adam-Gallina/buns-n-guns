using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : DirectInteraction
{
    protected override bool DoInteraction(Transform source)
    {
        LevelController.instance.PlayerWin();
        return true;
    }
}
