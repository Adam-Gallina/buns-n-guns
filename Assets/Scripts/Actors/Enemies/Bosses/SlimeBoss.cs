using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBoss : EnemyBase
{
    [Header("Combat")]
    [SerializeField] private float minAttackTime;
    [SerializeField] private float maxAttackTime;
    private float nextAttack;

    private SlimeBossAnim anim;

    protected override void Start()
    {
        base.Start();

        anim = GetComponentInChildren<SlimeBossAnim>();

        LevelController.instance.ui.bossHealthBar.AssignEntity(this, "Big Booty Bertha");
        LevelController.instance.ui.bossHealthBar.ShowHealthbar();
    }

    protected override void UpdateAttack()
    {
        if (Time.time >= nextAttack)
        {
            StartCoroutine(FireSequence());
        }
    }

    public void ConcentrationAnimEnd()
    {
        anim.Fire();
    }

    public void Fire()
    {
        
    }

    private IEnumerator FireSequence()
    {
        anim.Concentrate();
        canShoot = false;

        yield return new WaitUntil(() => anim.CheckFire());
        weapon.Aim(LevelController.instance.player.position);

        weapon.Fire();

        anim.Bounce();

        canShoot = true;
        nextAttack = Time.time + Random.Range(minAttackTime, maxAttackTime);
    }

    protected override void OnHit(Vector2 hitPos)
    {
        if (hitEffectPrefab)
            SpawnEffect(hitEffectPrefab, hitPos).SetEffectColor(hitCol);
        else
            Debug.LogWarning("Trying to spawn hit effect but hitEffectPrefab is not set");
    }

    public override void Death()
    {
        base.Death();

        LevelController.instance.ui.bossHealthBar.HideHealthbar();
    }
}
