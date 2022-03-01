using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBullet : BulletBase
{
    [Header("Raycast Bullet")]
    [SerializeField] protected float bulletDuration;
    [SerializeField] protected bool pierceTargets;

    protected LineRenderer line;

    protected override void Awake()
    {
        base.Awake();

        line = GetComponentInChildren<LineRenderer>();
        if (line)
        {
            line.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Can't find a line renderer in children");
        }
    }

    public override void SetBulletCol(Color targetColor)
    {
        line.startColor = targetColor;
        line.endColor = targetColor;
    }

    public override void SetVelocity(Vector2 targetVelocity)
    {
        DrawLine(targetVelocity.normalized);
    }

    protected void DrawLine(Vector2 dir)
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, dir, 100);

        if (hit.Length > 0)
        {
            int i;
            for (i = 0; i < hit.Length; i++)
            {
                if (HandleCollision(hit[i].collider) && !pierceTargets)
                    break;

                if (hit[i].collider.gameObject.tag == GameController.COLLIDABLE_TAG)
                    break;
            }

            line.SetPositions(new Vector3[]
            {
            new Vector3(transform.position.x, transform.position.y, -1),
            new Vector3(hit[i].point.x, hit[i].point.y, -1)
            });
            line.gameObject.SetActive(true);

            if (bulletDuration >= 0)
                Invoke("Despawn", bulletDuration);
        }
        else
        {
            Debug.Log("Something went wrong", this);
        }
    }

    public override void Despawn()
    {
        line.gameObject.SetActive(false);
        base.Despawn();
    }
}
