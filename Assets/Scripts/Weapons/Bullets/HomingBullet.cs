using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : BulletBase
{
    [Header("Targeting")]
    public float targetRange;
    public float adjustmentPower;

    protected override void Update()
    {
        if (LevelController.instance.paused)
            return;

        base.Update();

        CheckForEnemy();
    }

    private void CheckForEnemy()
    {
        Collider2D[] inRange = Physics2D.OverlapCircleAll(transform.position, targetRange, targetLayers);
        Collider2D closestObj = null;
        float closestDist = Mathf.Infinity;
        
        foreach (Collider2D target in inRange)
        {
            float dist = Vector2.Distance(transform.position, target.transform.position);
            if (dist < closestDist)
            {
                closestObj = target;
                closestDist = dist;
            }
        }

        if (closestObj)
        {
            Vector2 newDir = (closestObj.transform.position - transform.position).normalized;
            rb.AddForce(newDir * adjustmentPower, ForceMode2D.Force);
        }
    }
}
