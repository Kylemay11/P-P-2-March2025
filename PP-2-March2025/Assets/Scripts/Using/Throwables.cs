using UnityEngine;

[CreateAssetMenu(fileName = "Throwables", menuName = "Scriptable Objects/Throwables")]
public class Throwables : ScriptableObject
{
    public GameObject model;

    [Header("Basic Info")]
    public string itemName;
    [Range(5, 2000)] public int itemDamage;
    [Range(0, 1000)] public int itemDist;
    [Range(0.1f, 2)] public float itemPrimeRate;
    [Range(0.1f, 2)] public float itemThrowSpeed;
    [Range(0, 1000)] public int itemDamageRadius;


}
