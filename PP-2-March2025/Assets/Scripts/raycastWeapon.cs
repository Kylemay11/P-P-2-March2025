using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class raycastWeapon : MonoBehaviour
{
    public static raycastWeapon instance;

    [SerializeField] private float range;
    [SerializeField] public int damage;
    [SerializeField] public int MaxAmmo;
    [SerializeField] public int CurrentAmmo;
    [SerializeField] private float reloadTime;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float shootRate;
    [SerializeField] private GameObject muzzleFlash;

    private bool isReloading = false;
    private float nextShootTime;

    private int reloadStartWeaponIndex;
    void Start()
    {
        CurrentAmmo = MaxAmmo;

        if (AmmoUI.instance == null)
        {
            AmmoUI.instance = FindFirstObjectByType<AmmoUI>();
        }
        AmmoUI.instance.UpdateAmmo(CurrentAmmo, MaxAmmo);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && CurrentAmmo < MaxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    public void TryShoot()
    {
        if (isReloading) return;

        if (CurrentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Time.time >= nextShootTime)
        {
            Shoot();
            CurrentAmmo--;
            nextShootTime = Time.time + shootRate;
            AmmoUI.instance.UpdateAmmo(CurrentAmmo, MaxAmmo);
        }
    }

    private void Shoot()
    {
        if (muzzleFlash != null)
        {
            StartCoroutine(FlashMuzzle());
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 1.5f);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
        {
            Debug.Log("Hit: " + hit.collider.name);
            IDamage target = hit.collider.GetComponentInParent<IDamage>();
            if (target != null)
            {
                target.takeDamage((int)damage);
            }
        }
        else
        {
            Debug.Log("No Hit");
        }
    }

    private IEnumerator FlashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        AmmoUI.instance?.StartReload(reloadTime);

        float timer = 0f;
        while (timer < reloadTime)
        {

            timer += Time.deltaTime;
            
            yield return null;
        }

        CurrentAmmo = MaxAmmo;
        isReloading = false;

        AmmoUI.instance?.UpdateAmmo(CurrentAmmo, MaxAmmo);
    }

    public void ForceAmmoUIUpdate()
    {
        if (AmmoUI.instance != null)
        {
            AmmoUI.instance.UpdateAmmo(CurrentAmmo, MaxAmmo);
        }
    }


    // logic for shops
    public void DamageIncrease(int amount)
    {
        damage += amount;
    }

    //public void InfiniteAmmoPerk()
    //{
    //    MaxAmmo = int.MaxValue;
    //}

    public void AmmoIncrease(int amount)
    {
        CurrentAmmo += amount;
        AmmoUI.instance.UpdateAmmo(CurrentAmmo, MaxAmmo);
    }
}