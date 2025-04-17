using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] weaponStats wep;
    //[SerializeField] HealthPickups hpPack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        wep.ammoCur = wep.magCapacity;
        wep.totalAmmo = wep.initialReserveAmmo;
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickupable pickupable = other.GetComponent<IPickupable>();

        if (pickupable != null)
        {
            pickupable.getWeaponStats(wep);
            Destroy(gameObject);
        }
    }


}
