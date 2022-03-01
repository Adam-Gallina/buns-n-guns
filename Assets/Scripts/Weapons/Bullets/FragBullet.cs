using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragBullet : BulletBase
{
    [Header("Fragmentation")]
    [SerializeField] protected GameObject fragPrefab;
    [SerializeField] protected int fragCount;
    [SerializeField] protected float fragSpeed;

    protected float rotationAmount;

    protected override void Awake()
    {
        base.Awake();

        rotationAmount = 360 / fragCount;
    }

    protected BulletBase CreateFragBullet(GameObject prefab)
    {
        BulletBase newBullet = Instantiate(prefab, transform.position, transform.rotation).GetComponent<BulletBase>();
        newBullet.Setup(sourceController, damage, targetLayers);
        return newBullet;
    }

    public override void Despawn()
    {
        for (int i = 0; i < fragCount; i++)
        {
            BulletBase frag = CreateFragBullet(fragPrefab);
            frag.SetVelocity(MyDebug.Rotate(Vector2.right, rotationAmount * i) * fragSpeed);
        }

        base.Despawn();
    }
}
