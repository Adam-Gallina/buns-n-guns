using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBase : HealthBase
{
    [Header("Weapons")]
    [SerializeField] protected LayerMask targetLayers = 1 << GameController.PLAYER_LAYER;
    public GameObject DEBUG_startingWeapon;
    [SerializeField] public bool canUseWeapons = true;

    [Tooltip("The GameObject reflecting with the weapon")]
    [SerializeField] protected Transform weaponBody;

    [Tooltip("The GameObject rotating with the weapon")]
    [SerializeField] protected Transform weaponArm;

    [Tooltip("The GameObject parenting the weapon")]
    [SerializeField] protected Transform weaponHand;

    public float weaponScale = 1;
    protected WeaponBase weapon;
    protected bool canShoot = true;

    [Header("Collisions")]
    [SerializeField] protected int collisionDamage;
    [SerializeField] protected float collisionCooldown = 0.5f;
    protected float nextCollision;


    protected override void Awake()
    {
        base.Awake();

        if (weaponHand && canUseWeapons)
        {
            if (DEBUG_startingWeapon)
                AddWeapon(DEBUG_startingWeapon);
            else
            {
                WeaponBase weaponCheck = weaponHand.GetComponentInChildren<WeaponBase>();
                if (weaponCheck)
                    SetWeapon(weaponCheck);
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameController.INCORPOREAL_TAG))
            return;

        if ((1 << collision.gameObject.layer & targetLayers) != 0 && collisionDamage > 0)
        {
            DoCollisionDamage(collision.gameObject, collision.ClosestPoint(transform.position), collisionDamage);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameController.INCORPOREAL_TAG))
            return;

        if ((1 << collision.gameObject.layer & targetLayers) != 0 && collisionDamage > 0)
        {
            DoCollisionDamage(collision.gameObject, collision.ClosestPoint(transform.position), collisionDamage);
        }
    }

    public virtual void AddWeapon(GameObject weaponPrefab)
    {
        if (!canUseWeapons)
            return;

        if (!weaponBody)
        {
            Debug.LogWarning(name + " is trying to equip " + weaponPrefab.name + " but the weaponBody variable is not set!");
            return;
        }

        GameObject weapon = Instantiate(weaponPrefab, weaponHand);
        SetWeapon(weapon.GetComponent<WeaponBase>());
    }

    public virtual void SetWeapon(WeaponBase newWeapon)
    {
        if (!canUseWeapons)
            return;

        if (weapon)
            Destroy(weapon.gameObject);

        weapon = newWeapon;
        weapon.Setup(gameObject, weaponBody, weaponArm, targetLayers, weaponScale);
    }

    public override void SetAnimationState(bool state)
    {
        base.SetAnimationState(state);

        canShoot = !state;
    }

    protected virtual void AttemptCollisionDamage(GameObject target, Vector2 damagePoint)
    {
        if (collisionDamage == 0)
            return;

        if (Time.time < nextCollision)
            return;

        if (DoCollisionDamage(target, damagePoint, collisionDamage))
            nextCollision = Time.time + collisionCooldown;
    }

    protected virtual bool DoCollisionDamage(GameObject target, Vector2 damagePoint, int damage)
    {
        return target.GetComponentInParent<HealthBase>().Damage(gameObject, damagePoint, damage);
    }
}
