using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBullet : BulletBase
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (HandleCollision(collision))
        {
            SpawnEffect((collision.gameObject.layer & 1 << GameController.WALL_LAYER) != 0 ? destroyEffectPrefab : hitEffectPrefab,
                effectSpawn.position, destroyEffectCol);

            HealthBase hit = collision.GetComponentInParent<HealthBase>();
            if (hit)
                Instantiate(transform.GetChild(0).gameObject, transform.position, transform.rotation, hit.transform);
            else
                Instantiate(transform.GetChild(0).gameObject, transform.position, transform.rotation, collision.transform);
            Despawn();
        }
    }
}
