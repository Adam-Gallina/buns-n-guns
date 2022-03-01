using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletBase : MonoBehaviour
{
    protected GameObject sourceController;
    protected BulletPool sourcePool;

    [Header("Effects")]
    [SerializeField] protected GameObject spawnEffectPrefab;
    [SerializeField] protected Color spawnEffectCol = Color.white;
    //[SerializeField] protected GameObject travelEffectPrefab;
    //[SerializeField] protected Color travelEffectCol = Color.white;
    [SerializeField] protected GameObject hitEffectPrefab;
    [SerializeField] protected GameObject destroyEffectPrefab;
    [SerializeField] protected Color destroyEffectCol = Color.white;
    [SerializeField] protected Transform effectSpawn;

    protected int targetLayers;
    protected int damage = 1;

    protected Rigidbody2D rb;
    protected Transform image;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        image = transform.GetChild(0);
    }

    public virtual void Setup(GameObject sourceController, int targetLayers, Color bulletCol = new Color())
    {
        Setup(sourceController, 0, targetLayers, bulletCol);
    }
    public virtual void Setup(GameObject sourceController, int damage, int targetLayers, Color bulletCol = new Color())
    {
        if (bulletCol != new Color())
            SetBulletCol(bulletCol);

        SetBulletDamage(damage);

        this.sourceController = sourceController;
        this.targetLayers = targetLayers;
    }

    public void AddToBulletPool(BulletPool pool)
    {
        sourcePool = pool;
    }

    public virtual void SetBulletCol(Color targetColor)
    {
        image.GetComponent<SpriteRenderer>().color = targetColor;
    }

    public void SetBulletDamage(int damage)
    {
        this.damage = damage;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (HandleCollision(collision))
        {
            SpawnEffect((collision.gameObject.layer & 1 << GameController.WALL_LAYER) != 0 ? destroyEffectPrefab : hitEffectPrefab,
                effectSpawn.position, destroyEffectCol);
            Despawn();
        }
    }

    protected virtual void Update()
    {
        if (LevelController.instance.paused)
            return;

        UpdateRotation();
    }

    // Return True if the bullet collided (e.g. should be destroyed)
    protected virtual bool HandleCollision(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case GameController.INCORPOREAL_TAG:
                return false;
            case GameController.COLLIDABLE_TAG:
                return true;
            case GameController.DAMAGEABLE_TAG:
                if ((1 << collision.gameObject.layer & targetLayers) != 0)
                {
                    return DamageTarget(collision.gameObject, collision.ClosestPoint(transform.position));
                }
                return false;
            default:
                Debug.LogWarning("Don't know how to handle collision with " + collision.gameObject.tag + " tag");
                return false;
        }
    }

    // Returns whether or not the bullet 'hit' the target
    protected virtual bool DamageTarget(GameObject target, Vector2 damagePoint)
    {
        return target.GetComponentInParent<HealthBase>().Damage(sourceController, damagePoint, damage);
    }

    public virtual void SetVelocity(Vector2 targetVelocity)
    {
        rb.velocity = targetVelocity;

        UpdateRotation();
    }

    protected virtual void UpdateRotation()
    {
        transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, rb.velocity));
        image.localEulerAngles = new Vector3(rb.velocity.x < 0 ? 180 : 0, 0, 0);
    }

    public virtual void Spawn()
    {
        SpawnEffect(spawnEffectPrefab, effectSpawn.position, spawnEffectCol);
    }

    public virtual void Despawn()
    {
        rb.velocity = new Vector2();
        rb.angularVelocity = 0;
        transform.eulerAngles = new Vector3();
        if (sourcePool != null)
            sourcePool.ReturnBullet(gameObject);
        else
            Destroy(gameObject);
    }

    protected virtual EffectBase SpawnEffect(GameObject effectPrefab, Vector3 targetPos, Color effectCol)
    {
        if (!effectPrefab)
            return null;

        GameObject effect = Instantiate(effectPrefab);
        effect.transform.position = targetPos;
        effect.GetComponent<EffectBase>().SetEffectColor(effectCol);
        return effect.GetComponent<EffectBase>();
    }
}
