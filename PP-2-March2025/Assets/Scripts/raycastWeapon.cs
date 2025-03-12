using UnityEngine;

public class raycastWeapon : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask hitMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
        {
            Debug.Log("Hit: " + hit.collider.name);
            IDamage target = hit.collider.GetComponent<IDamage>();
            if (target != null)
                target.takeDamage((int)damage);
        }
        else
        {
            Debug.Log("No Hit");
        }
    }
}
