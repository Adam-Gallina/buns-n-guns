using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowSMG : WeaponBase
{
    [SerializeField] protected ColorBurst colorBurst;

    public override FiringMode[] GetFiringModes()
    {
        colorBurst.Setup(this, firePoint);
        return new FiringMode[] { colorBurst };
    }
}

[System.Serializable]
public class ColorBurst : Burst
{
    public Color[] colors;
    private int currCol = 0;

    public override IEnumerator FireBurst()
    {
        currCol = 0;
        return base.FireBurst();
    }

    protected override GameObject SpawnBullet(GameObject bulletPrefab, Vector2 firingDir, float spread = 0)
    {
        GameObject bullet = base.SpawnBullet(bulletPrefab, firingDir, spread);

        bullet.GetComponent<BulletBase>().SetBulletCol(colors[currCol++]);

        return bullet;
    }
}