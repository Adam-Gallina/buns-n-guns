using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOnlyPlayer : PlayerController
{
    private Vector2 currMovementTarget;
    [SerializeField] private float minDist = 0.1f;

    protected override void Start()
    {
        base.Start();

        currMovementTarget = transform.position;
    }

    protected override void UpdateMovement()
    {
        if (!canMove)
        {
            SetDir(Vector2.zero, 0);
            return;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            currMovementTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(transform.position, currMovementTarget) > minDist)
                MoveToTarget(currMovementTarget);
        }
        else
        {
            SetVelocity(Vector2.zero);
        }
    }
}
