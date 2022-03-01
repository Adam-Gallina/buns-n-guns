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

    public override float StartFire()
    {
        if (requireMouseUp && !mouseUp && firingComplete)
        {
            weapon.StartCoroutine(FireBurst());
            return fireDelay + (burstCount * burstShotDelay);
        }

        return 0;
    }

    public virtual IEnumerator FireBurst()
    {
        firingComplete = false;

        int totalShots = burstCount;
        while (totalShots > 0)
        {

            Fire();
            totalShots--;
            yield return new WaitForSeconds(burstShotDelay);
        }

        firingComplete = true;
    }
}
