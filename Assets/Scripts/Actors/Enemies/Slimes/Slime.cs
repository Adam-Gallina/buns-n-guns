using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : EnemyBase
{
    [Header("Image")]
    public float minColorBrightness;
    public float alpha = 200;
    protected Color spriteCol;

    [Header("Slime Pathing")]
    [SerializeField] protected bool trackPlayer = true;

    protected virtual Color GetSlimeCol()
    {
        return new Color(Random.Range(minColorBrightness, 255) / 255,
                         Random.Range(minColorBrightness, 255) / 255,
                         Random.Range(minColorBrightness, 255) / 255,
                         alpha / 255);
    }

    protected override Vector2 GetNextPoint()
    {
        if (noticedPlayer && trackPlayer)
            return LevelController.instance.player.position;

        return base.GetNextPoint();
    }

    protected override void OnSpawnStart()
    {
        spriteCol = GetSlimeCol();
        image.GetComponentInChildren<SpriteRenderer>().color = spriteCol;
        LoadSprites();

        base.OnSpawnStart();
    }

    protected override EffectBase DoSpawnEffect(Vector2 targetPos)
    {
        EffectBase effect = base.DoSpawnEffect(targetPos);
        effect.SetEffectColor(spriteCol);
        return effect;
    }

    protected override EffectBase DoDeathEffect(Vector2 targetPos)
    {
        EffectBase effect = base.DoDeathEffect(targetPos);
        effect.SetEffectColor(spriteCol);
        return effect;
    }
}
