using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawnerManager : MonoBehaviour
{
    [Header("Room Spawner Phase Settings")]
    [SerializeField] private List<ZombieSpawner> spawners = new List<ZombieSpawner>();
    [SerializeField] private int spawnersPerPhase;
    [SerializeField] private float phaseDuration;

    private int currentPhaseIndex = -1;
    private float phaseTimer;
    private bool isRoomWaveActive = false;

    [System.Serializable]
    public struct SpawnerPhase
    {
        public List<ZombieSpawner> spawners;
        public float duration;
    }

    private List<SpawnerPhase> roomPhases = new List<SpawnerPhase>();

    public void InitializeRoomWave(int waveNumber, float healthMult, float dmgMult, float speedMult)
    {
        foreach (var spawner in spawners)
        {
            spawner.SetCurrentWave(waveNumber, healthMult, dmgMult, speedMult);
            spawner.currentZombiesAlive = 0;
        }

        BuildRoomPhases();
        AdvanceRoomPhase();
        isRoomWaveActive = true;
    }

    private void Update()
    {
        if (!isRoomWaveActive || roomPhases.Count == 0) return;

        phaseTimer -= Time.deltaTime;
        if (phaseTimer <= 0f)
        {
            AdvanceRoomPhase();
        }

        // Active spawners spawn over time
        if (currentPhaseIndex >= 0 && currentPhaseIndex < roomPhases.Count)
        {
            foreach (var spawner in roomPhases[currentPhaseIndex].spawners)
            {
                spawner.SpawnOverTime();
            }
        }
    }

    private void AdvanceRoomPhase()
    {
        if (roomPhases == null || roomPhases.Count == 0)
        {
            Debug.LogWarning($"[RoomSpawnerManager] Tried to advance phase, but roomPhases is empty.");
            return;
        }

        StopAllSpawners();

        currentPhaseIndex = (currentPhaseIndex + 1) % roomPhases.Count;
        var phase = roomPhases[currentPhaseIndex];

        foreach (var spawner in phase.spawners)
        {
            spawner.StartSpawning();
        }

        phaseTimer = phase.duration;
    }

    private void StopAllSpawners()
    {
        foreach (var spawner in spawners)
        {
            spawner.StopSpawning();
        }
    }

    public void BuildRoomPhases()
    {
        roomPhases.Clear();

        List<ZombieSpawner> activeSpawners = new List<ZombieSpawner>();
        foreach (var spawner in spawners)
        {
            if (spawner != null && spawner.IsSpawnerActive())
                activeSpawners.Add(spawner);
        }

        int total = activeSpawners.Count;
        int numPhases = Mathf.CeilToInt((float)total / spawnersPerPhase);

        for (int i = 0; i < total; i += spawnersPerPhase)
        {
            SpawnerPhase phase = new SpawnerPhase
            {
                spawners = new List<ZombieSpawner>(),
                duration = phaseDuration
            };

            for (int j = 0; j < spawnersPerPhase && (i + j) < total; j++)
            {
                phase.spawners.Add(activeSpawners[i + j]);
            }

            roomPhases.Add(phase);
        }

        Debug.Log($"[RoomSpawnerManager] Built {roomPhases.Count} room phases with {spawnersPerPhase} spawners each.");
    }

    public int GetZombiesAlive()
    {
        int total = 0;
        foreach (var spawner in spawners)
        {
            total += spawner.currentZombiesAlive;
        }
        return total;
    }
    public void SetRoomActive(bool active)
    {
        isRoomWaveActive = active;
    }
    public void EndRoomWave()
    {
        isRoomWaveActive = false;
        StopAllSpawners();
        Debug.Log($"[RoomSpawnerManager] Room wave ended.");
    }
    public void SetRoomWaveActive()
    {
        isRoomWaveActive = true;
        Debug.Log($"[RoomSpawnerManager] Room wave manually set active for: {gameObject.name}");
    }
}
