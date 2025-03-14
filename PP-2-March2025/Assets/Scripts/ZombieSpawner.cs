using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{

    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> zombiePrefabs;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] public float spawnRate;
    [SerializeField] public int maxZombies;
    [SerializeField] private float spawnCooldown;

    public int currentZombiesAlive;
    private bool canSpawn ;

    public void StartSpawning()
    {
        canSpawn = true;
        StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        canSpawn = false;
    }

    IEnumerator SpawnLoop()
    {
        while (canSpawn && currentZombiesAlive < maxZombies)
        {
            SpawnZombie();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    void SpawnZombie()
    {
        if (currentZombiesAlive >= maxZombies) return;

        // Choose random zombie and spawn point
        GameObject zombiePrefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Count)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);

        currentZombiesAlive++;

        ZombieController controller = zombie.GetComponent<ZombieController>();
        if (controller != null)
        {
            controller.OnZombieDeath += HandleZombieDeath;
        }
    }

    void HandleZombieDeath()
    {
        currentZombiesAlive--;
    }

    public void SpawnOverTime()
    {
        if (spawnCooldown <= 0f)
        {
            SpawnZombie();
            spawnCooldown = spawnRate;
        }
        else
        {
            spawnCooldown -= Time.deltaTime;
        }
    }
}