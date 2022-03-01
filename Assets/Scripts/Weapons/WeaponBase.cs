using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    protected FiringMode[] firingModes;
    protected int lastMode;

    protected int targetLayers;
    protected GameObject controller;

    [Header("Image")]
    public Color bulletCol = Color.white;

    // Animations
    protected Transform linkedArm;
    protected Transform linkedBody;
    protected float armToGun;
    protected float gunAngleFromArm;
    protected bool lockRotation = false;

    protected Transform image;
    [SerializeField] protected Transform firePoint;

    protected virtual void Awake()
    {
        firingModes = GetFiringModes();
        image = transform.GetChild(0);
    }

    public abstract FiringMode[] GetFiringModes();

    public virtual void Setup(GameObject controller, Transform body, int targetLayers, float scale = 1)
    {
        Setup(controller, body, image, targetLayers, scale);
    }

    public virtual void Setup(GameObject controller, Transform body, Transform arm, int targetLayers, float scale = 1)
    {
        if (arm == null)
            arm = transform;

        this.controller = controller;
        linkedBody = body;
        linkedArm = arm;
        this.targetLayers = targetLayers;

        armToGun = (linkedArm.position - firePoint.position).magnitude;
        gunAngleFromArm = Vector2.Angle(linkedArm.position - firePoint.position, firePoint.right);

        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetRotationLockState(bool state)
    {
        lockRotation = state;
    }

    public virtual void Aim(Vector2 target)
    {
        if (lockRotation)
            return;

        Vector2 rightTarget = ReflectOverLinkedBody(target);
        Vector2 rightArm = ReflectOverLinkedBody(linkedArm.position);

        // Law of Sines
        float a = (rightTarget - rightArm).magnitude;
        float c = armToGun;
        float A = gunAngleFromArm;
        float C = Mathf.Asin(c / a * Mathf.Sin(A * Mathf.Deg2Rad)) * Mathf.Rad2Deg;

        float angle = -(C + Vector2.SignedAngle(rightTarget - rightArm, Vector2.right));
        if (float.IsNaN(angle))
            angle = Mathf.Abs(linkedArm.eulerAngles.z);

        if (target.x <= linkedBody.position.x)
        {
            linkedBody.localScale = new Vector3(-1, 1, 1);
            linkedArm.eulerAngles = new Vector3(0, 0, -angle);
            firePoint.localEulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            linkedBody.localScale = new Vector3(1, 1, 1);
            linkedArm.eulerAngles = new Vector3(0, 0, angle);
            firePoint.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    private Vector2 ReflectOverLinkedBody(Vector2 v)
    {
        Vector2 rightTarget = v - (Vector2)linkedBody.position;
        rightTarget.x = Mathf.Abs(rightTarget.x);
        rightTarget += (Vector2)linkedBody.position;
        return rightTarget;
    }

    public virtual void Fire()
    {
        int maxChance = 0;
        foreach (FiringMode m in firingModes)
            maxChance += m.chance;

        int numb = Random.Range(0, maxChance);
        int mode = 0;
        for (int i = 0; i < firingModes.Length; i++)
        {
            mode = i;
            numb -= firingModes[i].chance;
            if (numb <= 0)
                break;
        }

        Fire(mode);
    }

    public virtual void Fire(int firingMode)
    {
        if (firingMode < 0 || firingMode >= firingModes.Length)
        {
            Debug.LogWarning($"{controller.name} is trying to fire {name} with incorrect mode {firingMode}. Defaulting to mode 0 ({firingModes[0]})");
            firingMode = 0;
        }

        firingModes[firingMode].StartShooting();
        lastMode = firingMode;
    }

    public virtual void StopFire()
    {
        firingModes[lastMode].StopShooting();
    }

    public GameObject SetupNewBullet(GameObject prefab)
    {
        GameObject newBullet = Instantiate(prefab);
        newBullet.GetComponent<BulletBase>().Setup(controller, targetLayers, bulletCol);
        return newBullet;
    }
}

public class BulletPool
{
    private WeaponBase parent;
    private GameObject prefab;
    private List<GameObject> availableBullets = new List<GameObject>();

    public BulletPool(WeaponBase parent, GameObject bulletPrefab)
    {
        this.parent = parent;
        prefab = bulletPrefab;
    }

    public virtual GameObject GetBullet()
    {
        if (availableBullets.Count == 0)
        {
            GameObject newBullet = parent.SetupNewBullet(prefab);
            newBullet.GetComponent<BulletBase>().AddToBulletPool(this);
            availableBullets.Add(newBullet);
        }

        GameObject retBullet = availableBullets[0];
        availableBullets.RemoveAt(0);
        return retBullet;
    }

    public void ClearBulletPool()
    {
        foreach (GameObject bullet in availableBullets)
        {
            Object.Destroy(bullet);
        }
        availableBullets.Clear();
    }

    public virtual void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        availableBullets.Add(bullet);
    }
}

[System.Serializable]
public abstract class FiringMode
{
    public int chance;
    public GameObject bulletPrefab;
    public int damage = 1;
    public float bulletSpeed = 16;
    [Range(0, 90)] public float bulletSpread = 10;
    public float fireDelay = 0.25f;
    protected bool firingComplete = true;
    protected bool mouseUp = false;
    public bool requireMouseUp = false;
    public float nextShot = 0;

    protected WeaponBase weapon;
    protected Transform firePoint;
    protected BulletPool bulletPool;

    public void Setup(WeaponBase weapon, Transform firePoint)
    {
        this.weapon = weapon;
        this.firePoint = firePoint;

        bulletPool = new BulletPool(weapon, bulletPrefab);

        InitWeapon();
    }

    protected virtual void InitWeapon()
    {

    }

    protected virtual bool CanShoot()
    {
        return (!requireMouseUp || mouseUp)     // Player let go of fire button
            && firingComplete                   // Firing animation has finished
            && Time.time >= nextShot;           // Cooldown has passed
    }

    public virtual bool StartShooting()
    {
        if (!CanShoot())
            return false;

        FireBullet();
        nextShot = Time.time + fireDelay;

        mouseUp = false;

        return true;
    }
    
    public virtual bool StopShooting()
    {
        firingComplete = true;
        mouseUp = true;

        return true;
    }


    // How to fire the bullets
    protected virtual void FireBullet()
    {
        SpawnBullet(bulletPrefab, firePoint.right, bulletSpread);
    }

    public bool Completed()
    {
        return firingComplete;
    }

    // How to spawn a given bullet
    protected virtual GameObject SpawnBullet(GameObject bulletPrefab, Vector2 firingDir, float spread = 0)
    {
        if (spread != 0)
        {
            float angle = Random.Range(-spread, spread);
            firingDir = MyDebug.Rotate(firingDir, angle);
        }

        GameObject bullet = bulletPool.GetBullet();

        bullet.SetActive(true);
        bullet.transform.position = firePoint.position;
        BulletBase bBase = bullet.GetComponent<BulletBase>();
        bBase.SetVelocity(firingDir * bulletSpeed);
        bBase.SetBulletDamage(damage);
        bBase.Spawn();

        return bullet;
    }
}
