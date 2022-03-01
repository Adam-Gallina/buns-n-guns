using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongSniper : WeaponBase
{
    [SerializeField] protected DelayedShot delayedShot;

    public override FiringMode[] GetFiringModes()
    {
        delayedShot.Setup(this, firePoint);
        return new FiringMode[] { delayedShot };
    }
}

[System.Serializable]
public class DelayedShot : FiringMode
{
    [SerializeField] protected float shotDelay;

    public override float StartFire()
    {
        if (requireMouseUp && !mouseUp && firingComplete)
        {
            weapon.StartCoroutine(FireDelayed());
            return shotDelay + fireDelay;
        }

        return 0;
    }

    protected virtual IEnumerator FireDelayed()
    {
        firingComplete = false;
        weapon.SetRotationLockState(true);

        yield return new WaitForSeconds(shotDelay);

        Fire();

        firingComplete = true;
        weapon.SetRotationLockState(false);
    }
}