using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> zombiePrefabs;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private float spawnRate;
    [SerializeField] private int maxZombies;
    [SerializeField] private float spawnCooldown;

    [Header("Scaling Settings (per wave)")]
    [SerializeField] private float baseHealth;
    [SerializeField] private float baseDamage;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float healthGrowthRate;
    [SerializeField] private float damageGrowthRate;
    [SerializeField] private float speedGrowthRate;

    public int currentZombiesAlive;
    private bool canSpawn = false;
    private int currentWave = 1;

    public void SetCurrentWave(int waveNumber)
    {
        currentWave = waveNumber;
    }

    public void StartSpawning()
    {
        canSpawn = true;
        StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        canSpawn = false;
    }

    private IEnumerator SpawnLoop()
    {
        while (canSpawn && currentZombiesAlive < maxZombies)
        {
            SpawnZombie(currentWave);
            yield return new WaitForSeconds(spawnRate);
        }
    }

    public void SpawnZombie(int waveNumber)
    {
        if (currentZombiesAlive >= maxZombies || zombiePrefabs.Count == 0 || spawnPoints.Count == 0) return;

        GameObject zombiePrefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Count)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
        currentZombiesAlive++;

        float scaledHealth = baseHealth * Mathf.Pow(healthGrowthRate, waveNumber);
        float scaledDamage = baseDamage * Mathf.Pow(damageGrowthRate, waveNumber);
        float scaledSpeed = baseSpeed * Mathf.Pow(speedGrowthRate, waveNumber);

        IZombie zombieStats = zombie.GetComponent<IZombie>();
        if (zombieStats != null)
        {
            zombieStats.InitializeZombie(scaledHealth, scaledSpeed, scaledDamage);
        }

        ZombieController controller = zombie.GetComponent<ZombieController>();
        if (controller != null)
        {
            controller.OnZombieDeath += HandleZombieDeath;
        }
    }

    private void HandleZombieDeath()
    {
        currentZombiesAlive--;
    }

    public void SpawnOverTime()
    {
        if (!canSpawn) return;

        if (spawnCooldown <= 0f)
        {
            SpawnZombie(currentWave);
            spawnCooldown = spawnRate;
        }
        else
        {
            spawnCooldown -= Time.deltaTime;
        }
    }

}