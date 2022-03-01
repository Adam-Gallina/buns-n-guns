using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBullet : BulletBase
{
    [SerializeField] protected GameObject spawnPrefab;
    [SerializeField] protected GameObject spawnWeapon;

    public override void Despawn()
    {
        EnemyBase spawn = Instantiate(spawnPrefab, transform.position, Quaternion.identity).GetComponent<EnemyBase>();
        if (spawnWeapon)
            spawn.AddWeapon(spawnWeapon);
        spawn.InitializeEnemy();

        base.Despawn();
    }
}
