using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadWeapon : WeaponBase
{
    [SerializeField] protected Spread spread;

    public override FiringMode[] GetFiringModes()
    {
        spread.Setup(this, firePoint);
        return new FiringMode[] { spread };
    }
}

[System.Serializable]
public class Spread : Single
{
    public int shotCount;
    protected float spreadAngle;

    protected override void InitWeapon()
    {
        spreadAngle = bulletSpread * 2 / shotCount;
    }

    protected override void Fire()
    {
        firingComplete = false;

        for (int i = 0; i < shotCount; i++)
        {
            float angle = -bulletSpread + i * spreadAngle;
            Vector2 firingDir = MyDebug.Rotate(firePoint.right, angle);

            SpawnBullet(bulletPrefab, firingDir);
        }

        firingComplete = true;
    }
}