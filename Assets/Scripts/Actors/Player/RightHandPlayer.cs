using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandPlayer : PlayerController
{
    [Header("Automatic Targeting")]
    public float distBeforeChange = 1;
    private GameObject currTarget;

    protected override void Awake()
    {
        base.Awake();

        ic.left = "J";
        ic.right = "L";
        ic.up = "I";
        ic.down = "K";

        ic.interact = "U";
        ic.heal = "O";

        ic.fire = "Space";

        ic.UpdateKeys();
    }

    protected override Vector2 GetWeaponTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 50, targetLayers);

        if (enemies.Length > 0)
        {
            float closestDist = float.PositiveInfinity;
            int closestEnemy = 0;

            for (int i = 0; i < enemies.Length; i++)
            {
                float dist = Vector2.Distance(transform.position, enemies[i].transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestEnemy = i;
                }
            }

            if (currTarget)
            {
                if (Vector2.Distance(transform.position, currTarget.transform.position) - closestDist < distBeforeChange)
                    currTarget = enemies[closestEnemy].gameObject;
            }
            else
            {
                currTarget = enemies[closestEnemy].gameObject;
            }
        }
        
        return currTarget ? (Vector2)currTarget.transform.position : (Vector2)transform.position + rb.velocity;
    }
}
