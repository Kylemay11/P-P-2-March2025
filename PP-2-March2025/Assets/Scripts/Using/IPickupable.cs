using UnityEngine;

public interface IPickupable
{
    public void getWeaponStats(weaponStats wep);
    public void getThrowables(Throwables item);

    // public void OnHealthPickup(playerController player);
}
