using UnityEngine;
using System.Collections.Generic;

public class SpawnerZone : MonoBehaviour
{
    [Header("Spawners in this zone")]
    public List<ZombieSpawner> spawnersInZone;

    void Start()
    {
        ZoneManager.Instance?.RegisterZone(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ZoneManager.Instance?.EnterZone(this);
    }

    public void SetZoneSpawning(bool allowSpawning)
    {
        foreach (var spawner in spawnersInZone)
        {
            if (spawner != null)
                spawner.SetPlayerInZone(allowSpawning);
        }
    }
}
