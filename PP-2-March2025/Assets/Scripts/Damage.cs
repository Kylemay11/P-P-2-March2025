using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum damageType { moving, stationary, overTime };
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [Range(1, 10)][SerializeField] int damageAmount;
    [Range(1, 50)][SerializeField] float speed;
    [Range(1, 10)][SerializeField] int destroyTime;
    [Range(1, 4)][SerializeField] float damageTime;

    bool isDamage;

    void Start()
    {
        if (type == damageType.moving)
        {
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.isTrigger)
            return;

        IDamage damage = other.GetComponent<IDamage>();

        if (damage != null && (type == damageType.stationary || type == damageType.moving))
        {
            damage.takeDamage(damageAmount);
        }

        if (type == damageType.moving)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage damage = other.GetComponent<IDamage>();

        if (damage != null && type == damageType.overTime)
        {
            if (!isDamage)
                StartCoroutine(damageOther(damage));
        }

    }

    IEnumerator damageOther(IDamage d)
    {
        isDamage = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageTime);
        isDamage = false;
    }
}
