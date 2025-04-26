using System;
using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] weaponStats wep;
    [SerializeField] Throwables item;
    //[SerializeField] HealthPickups hpPack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        initializeWepStats();
        initializeThrowables();
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickupable pickupable = other.GetComponent<IPickupable>();

        if (pickupable != null)
        {
            (pickupable is IPickupable ? (Action)(() => pickupable.getWeaponStats(wep)) : () => pickupable.getThrowables(item))();
            Destroy(gameObject);
        }
    }

    private void initializeWepStats()
    {
        wep.ammoCur = wep.magCapacity;
        wep.curReserve = wep.initialReserveAmmo;
    }

    private void initializeThrowables()
    {
        item.curInventory = item.itemCapacity;
        item.curReserve = item.itemMaxCapacity;
    }
}
