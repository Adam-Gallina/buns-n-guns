using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class MovementBase : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 12;
    protected bool selfControlled = true;


    protected Rigidbody2D rb;
    protected AudioSource sound;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sound = GetComponent<AudioSource>();
    }

    public virtual void SetAnimationState(bool state)
    {
        selfControlled = !state;
    }

    public void SetDir(Vector2 targetDir, float speed = 0)
    {
        if (speed == 0)
            speed = moveSpeed;

        SetVelocity(targetDir.normalized * speed);
    }

    public void SetVelocity(Vector2 targetV)
    {
        rb.velocity = targetV;
    }

    // Returns True if the entity reached targetPos, False otherwise
    public virtual bool MoveToTarget(Vector2 targetPos, float speed = 0, float minDist = 0.25f, bool stopAtEnd = true)
    {
        if (speed == 0)
            speed = moveSpeed;

        Vector2 targetDir = (targetPos - (Vector2)transform.position).normalized;
        rb.velocity = targetDir * speed;

        bool arrived = Vector2.Distance(transform.position, targetPos) <= minDist;

        if (arrived)
            SetVelocity(Vector2.zero);

        return arrived;
    }
}

