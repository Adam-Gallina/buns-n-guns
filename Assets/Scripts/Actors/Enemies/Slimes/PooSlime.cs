using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooSlime : ChargeSlime
{
    [SerializeField] protected Gradient colors;

    protected override Color GetSlimeCol()
    {
        Color col = colors.Evaluate(Random.Range(0.00f, 1.00f));
        col.a = alpha / 255;
        return col;
    }
}
