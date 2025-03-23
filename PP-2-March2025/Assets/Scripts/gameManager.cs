
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] int frameRate;

    [Header("Menus")]
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuShop;
    [SerializeField] GameObject menuInventory;
    private GameObject menuActive;

    [Header("Wave Settings")]
    [SerializeField] public float waveDuration;
    [SerializeField] private List<ZombieSpawner> zombieSpawners;
    private float waveTimer;
    private bool waveActive;
    private bool waitingToStartWave = true;
    private int currentWave = 0;

    [Header("Wave Scaling Multipliers (Per Wave)")]
    [SerializeField] private float healthMultiplier;
    [SerializeField] private float damageMultiplier;
    [SerializeField] private float speedMultiplier;

    [Header("Player and UI")]
    public Image playerHPBar;
    public Image playerStaminaBar;
    public GameObject playerDamageScreen;
    public GameObject player;
    public playerController playerScript;
    public WeaponNotificationUI weaponNotification;
    public TMP_Text waveInfoText;
    public GameObject breakPanel;
    public TMP_Text startPromptText;

    [Header("Spawner Phase Settings")]
    [SerializeField] private List<SpawnerPhase> spawnerPhases;
    [SerializeField] private int spawnersPerPhase; // How many spawners per phase
    [SerializeField] private float PhaseDuration; // Duration per phase if not using waveDuration
    [SerializeField] private bool useWaveDurationSplit = false; // If true, divide waveDuration across phases
    [SerializeField] private bool isSpawnerActive = false; // If true, divide waveDuration across phases
    private int currentPhaseIndex;
    private float phaseTimer;

    [System.Serializable]
    public struct SpawnerPhase
    {
        public List<ZombieSpawner> spawners;
        public float duration;
    }

    public bool isPaused;
    private int goalCount;

    private void Awake()
    {
        Application.targetFrameRate = frameRate;
        instance = this;

        if (player == null) player = GameObject.FindWithTag("Player");
        if (player != null) playerScript = player.GetComponent<playerController>();
        if (weaponNotification == null) weaponNotification = FindAnyObjectByType<WeaponNotificationUI>();

        zombieSpawners = new List<ZombieSpawner>(FindObjectsByType<ZombieSpawner>(FindObjectsSortMode.None));

        waveInfoText.text = "";
        breakPanel.SetActive(false);
        ShowPreWavePrompt();
    }

    void Update()
    {
        HandlePauseInput();
        HandleWaveLogic();
        HandleSpawnerPhases();
    }

    private void HandlePauseInput()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                PauseGame();
                menuActive = menuPause;
                menuActive.SetActive(true);
                EventSystem.current.SetSelectedGameObject(menuPause.transform.GetChild(0).gameObject);
            }
            else if (menuActive == menuPause)
            {
                ResumeGame();
            }
        }
    }

    private void HandleWaveLogic()
    {
        if (waveActive)
        {
            waveTimer -= Time.deltaTime;
            UpdateWaveInfoText($"Wave {currentWave} | Time: {Mathf.CeilToInt(waveTimer)}s");

            if (waveTimer <= 0)
            {
                waveTimer = 0;
                waveActive = false;
                StopAllSpawners();
                UpdateAliveCounterUI();
                WaitBeforeCleanupUI();
            }

            if (currentPhaseIndex >= 0 && currentPhaseIndex < spawnerPhases.Count)
            {
                foreach (var spawner in spawnerPhases[currentPhaseIndex].spawners)
                {
                    spawner.SpawnOverTime();
                }
            }
        }
        else if (!waitingToStartWave && GetTotalZombiesAlive() <= 0)
        {
            UpdateWaveInfoText($"Wave {currentWave} Complete!");
            ShowPreWavePrompt();
        }
        else if (!waveActive && !waitingToStartWave)
        {
            UpdateAliveCounterUI();
        }

        if (waitingToStartWave && Input.GetKeyDown(KeyCode.F))
        {
            StartWave();
        }
    }


    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (menuPause != null)
        {
            menuPause.SetActive(true);
            menuActive = menuPause;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(menuPause.transform.GetChild(0).gameObject);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void StartWave()
    {
        currentWave++; // Increment wave
        

        currentPhaseIndex = -1;

        float healthMult = Mathf.Pow(healthMultiplier, currentWave);
        float dmgMult = Mathf.Pow(damageMultiplier, currentWave);
        float speedMult = Mathf.Pow(speedMultiplier, currentWave);

        foreach (var spawner in zombieSpawners)
        {
            if (spawner != null)
            {
                spawner.SetCurrentWave(currentWave, healthMult, dmgMult, speedMult);
                spawner.currentZombiesAlive = 0;
            }
        }
        AutoBuildSpawnerPhases();

        AdvanceSpawnerPhase();
        waveTimer = waveDuration;
        waveActive = true;
        waitingToStartWave = false;

        breakPanel.SetActive(false);
        startPromptText.text = "";

        ResumeGame();
    }

    private void ShowPreWavePrompt()
    {
        breakPanel.SetActive(true);
        startPromptText.text = $"Press F to start Wave {currentWave + 1}";
        waitingToStartWave = true;
        EventSystem.current.SetSelectedGameObject(menuPause.transform.GetChild(0).gameObject);
    }

    public void UpdateGameGoal(int amount)
    {
        goalCount += amount;
        if (goalCount <= 0)
        {
            PauseGame();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void YouLose()
    {
        PauseGame();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    private int GetTotalZombiesAlive()
    {
        int total = 0;
        foreach (var spawner in zombieSpawners)
        {
            total += spawner.currentZombiesAlive;
        }
        return total;
    }

    private void UpdateWaveInfoText(string text)
    {
        if (waveInfoText != null)
            waveInfoText.text = text;
    }
    private void UpdateAliveCounterUI()
    {
        UpdateWaveInfoText($"Wave {currentWave} | Cleanup Phase | Alive: {GetTotalZombiesAlive()}");
    }
    private void HandleSpawnerPhases()
    {
        if (!waveActive) return;
        if (spawnerPhases.Count == 0) return;

        phaseTimer -= Time.deltaTime;

        if (phaseTimer <= 0f)
        {
            AdvanceSpawnerPhase();
        }
    }

    private void AdvanceSpawnerPhase()
    {
        StopAllSpawners();

        currentPhaseIndex = (currentPhaseIndex + 1) % spawnerPhases.Count;
        SpawnerPhase currentPhase = spawnerPhases[currentPhaseIndex];

        if (currentPhase.spawners == null || currentPhase.spawners.Count == 0)
        {
            Debug.LogWarning($"SpawnerPhase {currentPhaseIndex} has no spawners assigned!");
            return;
        }

        foreach (var spawner in currentPhase.spawners)
        {
            spawner.StartSpawning();
        }

        phaseTimer = currentPhase.duration;
    }

    private void StopAllSpawners()
    {
        foreach (var spawner in zombieSpawners)
        {
            spawner.StopSpawning();
        }
    }
    private IEnumerator WaitBeforeCleanupUI()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateAliveCounterUI();
    }

    private void AutoBuildSpawnerPhases()
    {
        spawnerPhases.Clear();

        List<ZombieSpawner> activeSpawners = new List<ZombieSpawner>();
        foreach (var spawner in zombieSpawners)
        {
            if (spawner != null && spawner.IsSpawnerActive())
                activeSpawners.Add(spawner);
        }

        int totalSpawners = activeSpawners.Count;
        if (totalSpawners == 0)
        {
            Debug.LogWarning("No ACTIVE zombie spawners found. Cannot build phases.");
            return;
        }

        int numPhases = Mathf.CeilToInt((float)totalSpawners / spawnersPerPhase);

        for (int i = 0; i < totalSpawners; i += spawnersPerPhase)
        {
            SpawnerPhase phase = new SpawnerPhase
            {
                spawners = new List<ZombieSpawner>(),
                duration = useWaveDurationSplit ? (waveDuration / numPhases) : PhaseDuration
            };

            for (int j = 0; j < spawnersPerPhase && (i + j) < totalSpawners; j++)
            {
                phase.spawners.Add(activeSpawners[i + j]);
            }

            spawnerPhases.Add(phase);
        }

        Debug.Log($"[GameManager] Auto-built {spawnerPhases.Count} phases using {spawnersPerPhase} spawners per phase.");
    }

    public bool IsSpawnerActive()
    {
        return isSpawnerActive;
    }

    public void RegisterSpawner(ZombieSpawner spawner)
    {
        if (!zombieSpawners.Contains(spawner))
        {
            zombieSpawners.Add(spawner);
            Debug.Log($"Spawner registered: {spawner.name}");
        }
    }
}