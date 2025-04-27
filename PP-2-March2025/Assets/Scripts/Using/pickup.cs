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
        if(wep)
            initializeWepStats();
        if(item)
            initializeThrowables();
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickupable pickupable = other.GetComponent<IPickupable>();

        if (pickupable != null)
        {
            if (wep != null)
            {
                pickupable.getWeaponStats(wep);
            }
            else if (item != null)
            {
                pickupable.getThrowables(item);
            }
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
        if (!item.isPickedup)
        {
            item.curInventory = item.itemCapacity;
            item.curReserve = item.itemMaxCapacity;
            item.makePickupable();
        }
    }
}
