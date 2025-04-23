using UnityEngine;
using System.Collections.Generic;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;

    private List<SpawnerZone> allZones = new List<SpawnerZone>();
    private SpawnerZone currentZone;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterZone(SpawnerZone zone)
    {
        if (!allZones.Contains(zone))
            allZones.Add(zone);
    }

    public void EnterZone(SpawnerZone newZone)
    {
        if (currentZone == newZone) return;

        foreach (var zone in allZones)
        {
            zone.SetZoneSpawning(zone == newZone);
        }

        currentZone = newZone;
    }
}
