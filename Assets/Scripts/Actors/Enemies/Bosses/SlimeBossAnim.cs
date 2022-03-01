using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBossAnim : MonoBehaviour
{
    private bool fire = false;

    private SlimeBoss controller;

    private Animator anim;

    private void Awake()
    {
        controller = GetComponentInParent<SlimeBoss>();

        anim = GetComponent<Animator>();
    }

    public void Bounce()
    {
        anim.SetTrigger("End Fire");
        fire = false;
    }

    public void Concentrate()
    {
        anim.SetTrigger("Concentrate");
        fire = false;
    }

    public void Fire()
    {
        anim.SetTrigger("Fire");
        fire = false;
    }

    public void ConcentrationAnimEnd()
    {
        controller.ConcentrationAnimEnd();
    }

    public void Shoot()
    {
        controller.Fire();
        fire = true;
    }

    public bool CheckFire()
    {
        return fire;
    }
}
