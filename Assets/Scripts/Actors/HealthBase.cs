using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBase : MovementBase
{
    [Header("Health")]
    public int maxHealth;
    protected float currHealth;
    public float invincibilityTime;
    protected float nextHit;
    protected bool dead = false;
    protected bool canDamage = true;

    [Header("Effects")]
    [SerializeField] protected bool flashOnHit = true;
    [SerializeField] protected Color hitCol = Color.red;
    [SerializeField] protected GameObject spawnEffectPrefab;
    [SerializeField] protected GameObject hitEffectPrefab;
    [SerializeField] protected GameObject deathEffectPrefab;
    [SerializeField] protected Transform effectSpawn;
    
    [Header("Audio")]
    [SerializeField] protected SoundType hitSound;

    protected Transform image;
    protected SpriteRenderer[] colorFlashSprites;
    protected Color[] defaultSpriteColors;

    protected override void Awake()
    {
        base.Awake();

        image = transform.GetChild(0);
        if (!effectSpawn)
            effectSpawn = image;

        currHealth = maxHealth;
    }

    protected virtual void Start()
    {
        LoadSprites();
    }

    protected void LoadSprites()
    {
        if (GetComponentsInChildren<SpriteRenderer>().Length == 0)
            return;

        colorFlashSprites = GetComponentsInChildren<SpriteRenderer>();
        defaultSpriteColors = new Color[colorFlashSprites.Length];
        for (int i = 0; i < colorFlashSprites.Length; i++)
        {
            defaultSpriteColors[i] = colorFlashSprites[i].color;
        }
    }

    protected void ResetSpriteColors()
    {
        for (int i = 0; i < colorFlashSprites.Length; i++)
        {
            colorFlashSprites[i].color = defaultSpriteColors[i];
        }
    }

    protected void SetSpriteColors(Color targetCol)
    {
        foreach (SpriteRenderer s in colorFlashSprites)
            s.color = targetCol;
    }

    public override void SetAnimationState(bool state)
    {
        base.SetAnimationState(state);

        canDamage = !state;
    }

    public virtual int GetCurrHealth()
    {
        return (int)currHealth;
    }

    // Returns true if the hit was valid and false otherwise
    public virtual bool Damage(GameObject source, Vector2 damagePoint, int damage)
    {
        if (dead)
            return false;

        if (Time.time < nextHit || !canDamage)
        {
            return true;
        }

        currHealth -= damage;
        nextHit = Time.time + invincibilityTime;
        OnHit(damagePoint);
        if (currHealth <= 0)
        {
            Sounds.PlaySound(sound, hitSound);
            currHealth = 0;
            OnZeroHealth();
        }
        else
        {
            Sounds.PlaySound(sound, hitSound);
        }

        return true;
    }

    #region Callbacks
    protected virtual void OnSpawnStart()
    {
        image.gameObject.SetActive(false);
        SetAnimationState(true);

        if (spawnEffectPrefab)
            DoSpawnEffect(effectSpawn.position).SetCallback(OnSpawnComplete);
        else
            OnSpawnComplete();
    }

    protected virtual void OnSpawnComplete()
    {
        image.gameObject.SetActive(true);
        SetAnimationState(false);
    }

    protected virtual void OnHit(Vector2 hitPos)
    {
        if (flashOnHit)
            StartCoroutine(FlashCol(hitCol, GameController.hitColDuration));

        if (hitEffectPrefab)
            DoHitEffect(hitPos);
    }

    protected virtual void OnZeroHealth()
    {
        dead = true;
        if (deathEffectPrefab)
        {
            SetVelocity(Vector2.zero);
            SetAnimationState(true);
            image.gameObject.SetActive(false);

            if (deathEffectPrefab)
                DoDeathEffect(effectSpawn.position).SetCallback(Death);
        }
        else
        {
            Death();
        }
    }

    public virtual void Death()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Effects
    protected virtual IEnumerator FlashCol(Color col, float duration)
    {
        SetSpriteColors(col);
        yield return new WaitForSeconds(duration);
        ResetSpriteColors();
    }

    protected EffectBase SpawnEffect(GameObject effectPrefab, Vector3 targetPos)
    {
        GameObject effect = Instantiate(effectPrefab);
        effect.transform.position = targetPos;
        return effect.GetComponent<EffectBase>();
    }

    protected virtual EffectBase DoSpawnEffect(Vector2 targetPos)
    {
        return SpawnEffect(spawnEffectPrefab, targetPos);
    }

    protected virtual EffectBase DoDeathEffect(Vector2 targetPos)
    {
        return SpawnEffect(deathEffectPrefab, targetPos);
    }

    protected virtual EffectBase DoHitEffect(Vector2 targetPos)
    {
        return SpawnEffect(hitEffectPrefab, targetPos);
    }
    #endregion
}
