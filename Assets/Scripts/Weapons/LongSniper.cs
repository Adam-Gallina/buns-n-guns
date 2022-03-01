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

    public override bool StartShooting()
    {
        if (!CanShoot())
            return false;

        weapon.StartCoroutine(FireDelayed());
        nextShot = Time.time + shotDelay + fireDelay;
        return true;
    }

    protected virtual IEnumerator FireDelayed()
    {
        firingComplete = false;
        weapon.SetRotationLockState(true);

        yield return new WaitForSeconds(shotDelay);

        FireBullet();

        firingComplete = true;
        weapon.SetRotationLockState(false);
    }
}