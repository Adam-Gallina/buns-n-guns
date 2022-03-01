using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigun : WeaponBase
{
    public Automatic minigun;

    public override FiringMode[] GetFiringModes()
    {
        minigun.Setup(this, firePoint);
        return new FiringMode[] { minigun };
    }
}

[System.Serializable]
public class Automatic : DelayedShot
{
    protected override IEnumerator FireDelayed()
    {
        firingComplete = false;

        yield return new WaitForSeconds(shotDelay);

        while (!firingComplete)
        {
            Fire();
            yield return new WaitForSeconds(fireDelay);
        }

        firingComplete = true;
    }
}