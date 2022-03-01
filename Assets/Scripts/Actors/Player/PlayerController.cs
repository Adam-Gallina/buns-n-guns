using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(InputController))]
public class PlayerController : CombatBase
{
    static readonly float DIAGONAL_MODIFIER_CONST = 1 / Mathf.Sqrt(2);

    protected InputController ic;
    private Inventory inv;
    protected bool canMove = true;

    //[Header("Interactions")]
    private List<DirectInteraction> interactionQueue = new List<DirectInteraction>();
    private bool canInteract = true;

    private Animator anim;

    protected override void Awake()
    {
        base.Awake();

        image = transform.GetChild(0);

        ic = GetComponent<InputController>();
        inv = GetComponent<Inventory>();
        anim = image.GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        if (CameraController.instance)
        {
            CameraController.instance.SetFollowTarget(transform);
            CameraController.instance.TrackMouse(true);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == GameController.INTERACTIVE_LAYER &&
            collision.IsTouchingLayers(1 << GameController.PLAYER_INTERACTION_LAYER))
        {
            if (interactionQueue.Contains(collision.GetComponent<DirectInteraction>()))
                return;

            interactionQueue.Insert(0, collision.GetComponent<DirectInteraction>());
        }
        else
        {
            base.OnTriggerEnter2D(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == GameController.INTERACTIVE_LAYER &&
            !collision.IsTouchingLayers(1 << GameController.PLAYER_INTERACTION_LAYER))
        {
            interactionQueue.Remove(collision.GetComponent<DirectInteraction>());
        }
    }

    void Update()
    {
        if (!LevelController.instance.paused && !LevelController.instance.movementPaused)  
        {
            if (selfControlled)
            {
                UpdateMovement();
                if (canShoot)
                    UpdateWeapon();
                if (canInteract)
                    UpdateInput();
            }
        }

        UpdateAnim();
    }

    protected virtual void UpdateMovement()
    {
        if (!canMove)
        {
            SetDir(Vector2.zero, 0);
            return;
        }

        float x = 0, y = 0;

        if (ic.left) x -= 1;
        if (ic.right) x += 1;

        if (ic.up) y += 1;
        if (ic.down) y -= 1;

        if (x != 0 && y != 0)
        {
            x *= DIAGONAL_MODIFIER_CONST;
            y *= DIAGONAL_MODIFIER_CONST;
        }

        SetDir(new Vector2(x, y), moveSpeed);
    }

    protected virtual void UpdateWeapon()
    {
        if (!weapon)
            return;

        weapon.Aim(GetWeaponTarget());

        if (ic.fire.down || ic.fire.held)
        {
            weapon.Fire();
        }
        else if (ic.fire.up)
        {
            weapon.StopFire();
        }
    }

    protected virtual void UpdateInput()
    {
        if (ic.interact.down && interactionQueue.Count > 0)
            interactionQueue[0].Interact(transform);

        if (ic.heal.down)
            AttemptHeal();
    }

    protected virtual Vector2 GetWeaponTarget()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void AttemptHeal()
    {
        if (currHealth >= maxHealth)
        {
            FloatingText.instance.CreateText(transform.position, "Health is full");
            return;
        }

        InventoryItem usedPotion = inv.GetItem(Inventory.HealthPotion);
        if (usedPotion)
        {
            inv.RemoveItem(usedPotion);
            currHealth += ((HealthPotion)usedPotion).health;
            currHealth = Mathf.Clamp(currHealth, 0, maxHealth);
        }
        else
        {
            FloatingText.instance.CreateText(transform.position, "Out of potions");
        }
    }

    private void UpdateAnim()
    {
        anim.speed = LevelController.instance.paused ? 0 : 1;

        anim.SetBool("Walking", rb.velocity != Vector2.zero);
    }

    public override void Death()
    {
        LevelController.instance.PlayerDeath();
    }

}
