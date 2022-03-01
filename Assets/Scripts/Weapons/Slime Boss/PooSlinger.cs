using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooSlinger : WeaponBase
{
    [SerializeField] protected Single singlePoo;
    [SerializeField] protected Single singleSlime;
    [SerializeField] protected Ring pooRing;
    [SerializeField] protected Burst slimeBurst;

    public override FiringMode[] GetFiringModes()
    {
        singlePoo.Setup(this, firePoint);
        singleSlime.Setup(this, firePoint);
        pooRing.Setup(this, firePoint);
        slimeBurst.Setup(this, firePoint);

        return new FiringMode[] { singlePoo, singleSlime, pooRing, slimeBurst };
    }
}

[System.Serializable]
public class Ring : Spread
{
    protected override void InitWeapon()
    {
        spreadAngle = 360 / shotCount;
    }

    protected override void Fire()
    {
        float startRotation = Random.Range(0, spreadAngle);

        for (int i = 0; i < shotCount; i++)
        {
            float angle = -bulletSpread + i * spreadAngle;
            Vector2 firingDir = MyDebug.Rotate(firePoint.right, startRotation + angle);

            SpawnBullet(bulletPrefab, firingDir);
        }
    }
}