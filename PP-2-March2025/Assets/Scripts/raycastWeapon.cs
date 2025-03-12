using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class raycastWeapon : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private float damage;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int currentAmmo;
    [SerializeField] private float reloadTime;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float shootRate;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] public AmmoUI ammoUI;
    [SerializeField] private Image reloadIndicator;

    private bool isReloading = false;
  


    private float nextShootTime;

     void Start()
    {
        currentAmmo = maxAmmo;

        if (ammoUI == null)
        {
            ammoUI = FindObjectOfType<AmmoUI>();
        }

        ammoUI.UpdateAmmo(currentAmmo, maxAmmo);
    }

    private T FindObjectsByType<T>()
    {
        throw new NotImplementedException();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }
    public void TryShoot()
    {
        if (isReloading) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Time.time >= nextShootTime)
        {
            Shoot();
            currentAmmo--;
            nextShootTime = Time.time + shootRate;
            ammoUI.UpdateAmmo(currentAmmo, maxAmmo);
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
            IDamage target = hit.collider.GetComponent<IDamage>();
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

        if (reloadIndicator != null)
            reloadIndicator.fillAmount = 0f;

        float timer = 0f;

        while (timer < reloadTime)
        {
            timer += Time.deltaTime;
            if (reloadIndicator != null)
                reloadIndicator.fillAmount = timer / reloadTime;
            yield return null;
        }

        currentAmmo = maxAmmo;
        isReloading = false;

        if (reloadIndicator != null)
            reloadIndicator.fillAmount = 0f;

        if (ammoUI != null)
            ammoUI.UpdateAmmo(currentAmmo, maxAmmo);
    }
    public void ForceAmmoUIUpdate()
    {
        if (ammoUI != null)
        {
            ammoUI.UpdateAmmo(currentAmmo, maxAmmo);
        }
    }
    public void SetAmmoUI(AmmoUI ui)
    {
        ammoUI = ui;
    }
}