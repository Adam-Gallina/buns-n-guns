using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : WeaponBase
{
    [SerializeField] protected Single singleShot;

    public override FiringMode[] GetFiringModes()
    {
        singleShot.Setup(this, firePoint);
        return new FiringMode[] { singleShot };
    }
}

[System.Serializable]
public class Single : FiringMode
{
}