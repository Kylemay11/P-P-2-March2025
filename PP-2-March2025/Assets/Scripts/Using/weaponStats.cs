using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon Stats")]

public class weaponStats : ScriptableObject
{
    public GameObject model;

    [Header("Basic Info")]
    public string weaponName;

    [Range(5, 2000)] public int wepDamage;
    [Range(0, 1000)] public int wepDist;
    [Range(0.1f, 2)] public float wepRate;
    public int ammoCur;
    public int curReserve;
    [Range(0, 500)] public int magCapacity;
    [Range(0, 5000)] public int initialReserveAmmo;
    [Range(0.0f, 5)] public float reloadTime;

    public ParticleSystem hitEffect;
    public AudioClip[] wepSound;
    [Range(0.0f, 1)] public float wepVolume;

    public bool isMelee;

    public void Reload()
    {
        int ammoNeeded = magCapacity - ammoCur;
        // checks reserve to see if you can fully reload
        int ammoToAdd = Mathf.Min(ammoNeeded, curReserve);

        ammoCur += ammoToAdd;
        curReserve -= ammoToAdd;
    }

    public bool CanFire()
    {
        return ammoCur > 0;
    }    

    public void maxAmmo()
    {
        ammoCur = magCapacity;
        curReserve = initialReserveAmmo;
    }
}
