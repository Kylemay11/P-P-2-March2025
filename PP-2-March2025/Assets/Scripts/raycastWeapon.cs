using System.Collections;
using UnityEngine;

public class raycastWeapon : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float shootRate;
    [SerializeField] private GameObject muzzleFlash;

    private float nextShootTime;
    public void TryShoot()
    {
        if (Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + shootRate;
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
}


