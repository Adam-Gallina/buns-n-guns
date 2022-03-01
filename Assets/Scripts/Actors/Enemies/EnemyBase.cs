using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBase : CombatBase
{
    [Header("Pathing")]
    public float minTargetRange;
    public float maxTargetRange;
    protected Vector2 nextPoint;
    protected bool canMove = true;

    [Header("Combat")]
    [SerializeField] protected float firstAttackDelay = 1;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float maxAttackRange;
    protected bool noticedPlayer = false;

    // Callbacks
    protected EnemyRoom spawnSource;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minTargetRange);
        Gizmos.DrawWireSphere(transform.position, maxTargetRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    protected override void Awake()
    {
        base.Awake();

        image = transform.GetChild(0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GameController.COLLIDABLE_TAG) ||
            collision.gameObject.CompareTag(GameController.DAMAGEABLE_TAG))
        {
            nextPoint = GetNextPoint();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GameController.COLLIDABLE_TAG) ||
            collision.gameObject.CompareTag(GameController.DAMAGEABLE_TAG))
        {
            nextPoint = GetNextPoint();
        }
    }

    protected virtual void Update()
    {
        if (LevelController.instance.movementPaused || LevelController.instance.paused)
            return;

        CheckForPlayer();
        if (canMove && selfControlled)
        {
            if (MoveToTarget(nextPoint, moveSpeed))
            {
                nextPoint = GetNextPoint();
            }
        }
        if (canShoot && selfControlled)
            UpdateAttack();
    }

    protected virtual void CheckForPlayer()
    {
        // Check if there's a wall between player and enemy
        if (Physics2D.Linecast(transform.position, LevelController.instance.player.position, 1 << GameController.WALL_LAYER))
        {
            noticedPlayer = false;
        }
        else 
        {
            if (Physics2D.OverlapCircle(transform.position, attackRange, 1 << GameController.PLAYER_LAYER))
            {
                noticedPlayer = true;
            }
            else if (!Physics2D.OverlapCircle(transform.position, maxAttackRange, 1 << GameController.PLAYER_LAYER))
            {
                noticedPlayer = false;
            }
        }
    }

    protected virtual void UpdateAttack()
    {
        if (weapon)
        {
            if (noticedPlayer)
            {
                weapon.Aim(LevelController.instance.player.position);
                weapon.Fire();
            }
            else
            {
                weapon.Aim(nextPoint);
                weapon.StopFire();
            }
        }
    }

    protected virtual Vector2 GetNextPoint()
    {
        return GetRandomPoint();
    }

    protected virtual Vector2 GetRandomPoint()
    {
        float targetDist = Random.Range(minTargetRange, maxTargetRange);
        Vector2 targetPoint = (Vector2)transform.position + Random.insideUnitCircle.normalized * targetDist;

        return targetPoint;
    }

    public override bool Damage(GameObject source, Vector2 damagePoint, int damage)
    {
        noticedPlayer = true;
        return base.Damage(source, damagePoint, damage);
    }

    public override void Death()
    {
        if (spawnSource)
            spawnSource.OnEnemyDeath(gameObject);
        base.Death();
    }

    public virtual void InitializeEnemy()
    {
        OnSpawnStart();
    }
    public virtual void InitializeEnemy(EnemyRoom spawnSource)
    {
        this.spawnSource = spawnSource;
        OnSpawnStart();
    }

    protected override void OnSpawnComplete()
    {
        base.OnSpawnComplete();

        nextPoint = GetNextPoint();

        canShoot = false;
        Invoke("AllowAttacking", firstAttackDelay);
    }

    protected void AllowAttacking()
    {
        canShoot = true;
    }
}
