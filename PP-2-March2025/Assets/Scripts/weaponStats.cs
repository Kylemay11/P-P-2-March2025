using UnityEngine;

[CreateAssetMenu]

public class weaponStats : ScriptableObject
{
    public GameObject model;
    [Range(1, 2000)] public int wepDamage;
    [Range(5, 1000)] public int wepDist;
    [Range(0.1f, 2)] public float wepRate;
    public int ammoCur;
    public int curReserve;
    [Range(1, 500)] public int magCapacity;
    [Range(10, 5000)] public int initialReserveAmmo;
    [Range(0.1f, 5)] public float reloadTime;

    public ParticleSystem hitEffect;
    public AudioClip[] wepSound;
    [Range(0, 1)] public float wepVolume;

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

}
