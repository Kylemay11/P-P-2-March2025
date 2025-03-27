using UnityEngine;

[CreateAssetMenu]

public class weaponStats : ScriptableObject
{
    public GameObject model;
    [Range(1, 2000)] public int wepDamage;
    [Range(5, 1000)] public int wepDist;
    [Range(0.1f, 2)] public float wepRate;
    public int ammoCur;
    [Range(1, 100)] public int ammoMax;
    [Range(100, 300)] public int totalAmmo;
    [Range(0.1f, 5)] public float reloadTime;

    public ParticleSystem hitEffect;
    public AudioClip[] wepSound;
    [Range(0, 1)] public float wepVolume;

    public void DamageIncrease(int amount)
    {
        wepDamage += amount;
    }
}
