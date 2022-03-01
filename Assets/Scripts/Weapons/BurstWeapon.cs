using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstWeapon : WeaponBase
{
    [SerializeField] protected Burst burst;

    public override FiringMode[] GetFiringModes()
    {
        burst.Setup(this, firePoint);
        return new FiringMode[] { burst };
    }
}

[System.Serializable]
public class Burst : FiringMode
{
    public int burstCount;
    public float burstShotDelay;

    public override bool StartShooting()
    {
        if (!CanShoot())
            return false;

        weapon.StartCoroutine(FireBurst());
        nextShot = Time.time + burstShotDelay * burstCount + fireDelay;
        return true;
    }

    public virtual IEnumerator FireBurst()
    {
        firingComplete = false;

        int totalShots = burstCount;
        while (totalShots > 0)
        {

            FireBullet();
            totalShots--;
            yield return new WaitForSeconds(burstShotDelay);
        }

        firingComplete = true;
    }
}
