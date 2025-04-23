using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnerDifficulty { Easy, Medium, Hard }

public class ZombieSpawner : MonoBehaviour
{

    [Header("Spawn Settings")]
    [SerializeField] private SpawnerDifficulty difficulty;
    [SerializeField] private List<GameObject> allZombieTypes;
    [SerializeField] private List<GameObject> zombiePrefabs;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private float spawnRate;
    [SerializeField] private int maxZombies;
    [SerializeField] private float spawnCooldown;
    [SerializeField] public bool isSpawnerActiveInternal;
    [SerializeField] public BarricadeDoor barricadeDoor;
    private bool playerInZone = true;


    public int currentZombiesAlive;
    private bool canSpawn = false;
    private int currentWave = 1;
    private Color SpawnerdiffColor = Color.white;
    public SpawnerDifficulty Difficulty => difficulty;

    private float healthMultiplier;
    private float damageMultiplier;
    private float speedMultiplier;

    private Coroutine spawnCoroutine;

    public void SetCurrentWave(int wave, float healthMult, float dmgMult, float speedMult)
    {
        currentWave = wave;
        this.healthMultiplier = healthMult;
        this.damageMultiplier = dmgMult;
        this.speedMultiplier = speedMult;

        zombiePrefabs.Clear();

        for (int i = 0; i < allZombieTypes.Count; i++)
        {
            if (wave >= (i + 1))
            {
                zombiePrefabs.Add(allZombieTypes[i]);
            }
        }
    }

    public void StartSpawning()
    {
        canSpawn = true;
        // Just in case it’s lingering
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        canSpawn = false;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (canSpawn && currentZombiesAlive < maxZombies)
        {
            if (gameManager.instance.waveActive && playerInZone)
            {
                SpawnZombie();
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    public void SpawnZombie()
    {
        if (currentZombiesAlive >= maxZombies || zombiePrefabs.Count == 0 || spawnPoints.Count == 0) return;

        GameObject zombiePrefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Count)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
        currentZombiesAlive++;

        IZombie zombieStats = zombie.GetComponent<IZombie>();
        if (zombieStats != null)
        {
            zombieStats.InitializeZombie(healthMultiplier, speedMultiplier, damageMultiplier);
        }

        bool deathHooked = false;

        enemyAI enemy = zombie.GetComponent<enemyAI>();
        if (enemy != null)
        {
            enemy.barrierDoor = spawnPoint.GetComponentInChildren<BarricadeDoor>();
            enemy.OnZombieDeath += HandleZombieDeath;
            enemy.originSpawner = this;
            deathHooked = true;
        }
        if (RepairConsole.mainTerminalRef != null && RepairConsole.mainTerminalRef.doorCharging)
        {
            enemy.SetTargetState(enemyAI.ZombieTargetState.AttackingPlayer); // this ensures no door confusion
            enemy.GoToTerminal(RepairConsole.mainTerminalRef.transform.position);
            Debug.Log($"[Zombie Target] {zombie.name} set to attack main terminal.");
        }

        //BossAI boss = zombie.GetComponent<BossAI>();
        //if (boss != null)
        //{
        //    boss.OnZombieDeath += HandleZombieDeath;
        //    deathHooked = true;
        //}

        if (!deathHooked)
        {
            Debug.LogWarning($"{zombie.name} was spawned but has no valid death event!");
        }
    }

    private void HandleZombieDeath()
    {
        currentZombiesAlive--;
    }

    public void SpawnOverTime()
    {
        if (!canSpawn || !playerInZone || !gameManager.instance.waveActive) return;

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

    public bool IsSpawnerActive()
    {
        return isSpawnerActiveInternal;
    }

    public void SetSpawnerActive(bool active)
    {
        isSpawnerActiveInternal = active;

        if (active && difficulty == default) // assign if not already assigned
            AssignRandomDifficulty();
    }

    public void AssignRandomDifficulty()
    {
        difficulty = (SpawnerDifficulty)Random.Range(0, System.Enum.GetValues(typeof(SpawnerDifficulty)).Length);
        Debug.Log($"[Spawner] {gameObject.name} assigned difficulty: {difficulty}");
        ApplyDifficultySettings();
    }

    private void ApplyDifficultySettings()
    {
        switch (difficulty)
        {
            case SpawnerDifficulty.Easy:
                spawnRate = 5f;
                maxZombies = 1;
                spawnCooldown = 2f;
                SpawnerdiffColor = Color.green;
                break;

            case SpawnerDifficulty.Medium:
                spawnRate = 3f;
                maxZombies = 2;
                spawnCooldown = 1.5f;
                SpawnerdiffColor = Color.yellow;
                break;

            case SpawnerDifficulty.Hard:
                spawnRate = 1.5f;
                maxZombies = 3;
                spawnCooldown = 1f;
                SpawnerdiffColor = Color.red;
                break;
        }

        Debug.Log($"[Spawner] {gameObject.name} settings applied for {difficulty}: Rate={spawnRate}, Max={maxZombies}, Cooldown={spawnCooldown}");
    }
    public void SetPlayerInZone(bool value)
    {
        playerInZone = value;

        if (!playerInZone)
        {
            StopSpawning();
        }
        else if (isSpawnerActiveInternal)
        {
            StartSpawning();
        }
    }


    private void OnDrawGizmos() // for changing the color in the scene veiw only
    {
        Gizmos.color = SpawnerdiffColor;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.5f, 0.75f);
    }
}