using UnityEngine;

[CreateAssetMenu(fileName = "Throwables", menuName = "Scriptable Objects/Throwables")]
public class Throwables : ScriptableObject
{
    public GameObject model;
    public GameObject itemPrefab;

    [Header("Basic Info")]
    public string itemName;
    [Range(5, 2000)] public int itemDamage;
    [Range(0, 1000)] public int itemDist; // how far you can throw
    [Range(0.1f, 2)] public float itemPrimeRate; // pull pin or light cloth
    [Range(0.1f, 5)] public float itemDelay; // time before explosion
    [Range(0.1f, 200)] public float itemThrowForce; // velocity of projectile
    [Range(0, 1000)] public int itemDamageRadius; // range of AOE

    [Header("UI Info")]
    public int curInventory;
    public int curReserve;
    [Range(0, 1)] public int itemCapacity; //hold 1 grenade at a time
    [Range(0, 9)] public int itemMaxCapacity;
    [Range(0.0f, 5)] public float reloadTime;

    [Header("VFX")]
    public ParticleSystem hitEffect;

    [Header("Audio")]
    public AudioClip[] itemSound;
    [Range(0.0f, 1)] public float itemVolume;

    float explosionCountDown;
    public bool isPickedup = false;
    public bool IsThrowable;

    public bool CanThrow()
    {
        return curInventory > 0;
    }
    public bool CanReload()
    {
        return curReserve > 0;
    }

    public void pickedup()
    {
        if (isPickedup)        
            itemPrefab.GetComponent<SphereCollider>().enabled = false;  
    }

    public void makePickupable()
    {
        if(!isPickedup)
            itemPrefab.GetComponent<SphereCollider>().enabled = true;
    }

    public void Reload()
    {
        if (CanReload())
        {
            // always increments of 1
            curInventory += itemCapacity;
            curReserve--;

            //AmmoUI.instance?.UpdateThrowable(curInventory, curReserve);
        }
        else
        {
          // last throwable sound??  
        } 
    }

}
