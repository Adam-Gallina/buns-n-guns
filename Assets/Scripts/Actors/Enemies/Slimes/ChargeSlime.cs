using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeSlime : Slime
{
    [Header("Charge Attack")]
    [SerializeField] protected float chargeDelay;
    [SerializeField] protected float chargeDistance;
    [SerializeField] protected float chargeSpeed;
    [SerializeField] protected int chargeDamage;
    protected bool canCharge = true;
    protected bool charging = false;

    protected override void UpdateAttack()
    {
        if (noticedPlayer)
        {
            if (canCharge)
            {
                Vector2 chargeDir = (LevelController.instance.player.position - transform.position).normalized;
                Vector2 targetPos = (Vector2)transform.position + chargeDir * chargeDistance;
                StartCoroutine(DoCharge(targetPos, chargeSpeed, chargeDelay, chargeDelay));
            }
        } 
    }

    protected virtual IEnumerator DoCharge(Vector2 targetPos, float chargeSpeed, float preChargeDelay, float postChargeDelay)
    {
        canCharge = false;
        canMove = false;
        rb.velocity = new Vector2();
        yield return new WaitForSeconds(preChargeDelay);

        charging = true;
        yield return new WaitUntil(() => MoveToTarget(targetPos, chargeSpeed));
        charging = false;
        rb.velocity = new Vector2();

        yield return new WaitForSeconds(postChargeDelay);
        canCharge = true;
        canMove = true;
        noticedPlayer = false;
    }

    protected override bool DoCollisionDamage(GameObject target, Vector2 damagePoint, int damage)
    {
        if (charging)
            return target.GetComponentInParent<HealthBase>().Damage(gameObject, damagePoint, chargeDamage);

        return base.DoCollisionDamage(target, damagePoint, damage);
    }
}
