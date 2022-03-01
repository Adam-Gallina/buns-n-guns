using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : BulletBase
{
    protected Vector2 accelDir;
    [SerializeField] protected float accelMod = 1;

    protected override void Update()
    {
        base.Update();

        rb.AddForce(accelDir * accelMod);
    }

    public override void SetVelocity(Vector2 targetVelocity)
    {
        accelDir = targetVelocity;
    }
}
