
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;
    [SerializeField] int frameRate;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [Header("Wave Settings")]
    [SerializeField] public float waveDuration;
    [SerializeField] private float waveTimer;
    [SerializeField] private bool waveActive;
    [SerializeField] private int currentWave;

    [SerializeField] public List<ZombieSpawner> zombieSpawners;

    public Image playerHPBar;
    public Image playerStaminaBar;
    public GameObject playerDamageScreen;
    public GameObject player;
    public playerController playerScript;
    public WeaponNotificationUI weaponNotification;
    public TMP_Text waveInfoText;
    public GameObject breakPanel;
    public TMP_Text startPromptText;

    public bool isPaused;
    private bool waitingToStartWave;

    int goalCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (frameRate <= 0) frameRate = 60;
        Application.targetFrameRate = frameRate;
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        weaponNotification = FindAnyObjectByType<WeaponNotificationUI>();
        waveInfoText.text = "";
        breakPanel.SetActive(false);
        waitingToStartWave = true;
        EnterPreWaveState();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpaused();
            }
        }

        if (waveActive)
        {
            waveTimer -= Time.deltaTime;
            waveInfoText.text = $"Wave {currentWave + 1} | Time: {Mathf.CeilToInt(waveTimer)}s";

            if (waveTimer <= 0)
            {
                waveTimer = 0;
                waveActive = false;
                waveInfoText.text = $"Wave {currentWave + 1} | Cleanup Phase | Alive: {GetTotalZombiesAlive()}";
                Debug.Log("Wave timer ended! Waiting for all zombies to be cleared...");
            }

            foreach (var spawner in zombieSpawners)
            {
                spawner.SpawnOverTime();
            }
        }
        else if (!waitingToStartWave)
        {
            if (GetTotalZombiesAlive() <= 0)
            {
                Debug.Log("Wave Complete!");
                currentWave++;
                waveInfoText.text = $"Wave {currentWave} Complete!";
                EnterPreWaveState();
            }
        }

        if (waitingToStartWave && Input.GetKeyDown(KeyCode.F))
        {
            StartWave();
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void updateGameGoal(int amount)
    {
        goalCount += amount;

        if (goalCount <= 0)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void StartWave()
    {
        waveTimer = waveDuration;
        waveActive = true;
        waitingToStartWave = false;

        Debug.Log("Wave " + (currentWave + 1) + " started!");

        if (playerScript != null)
            playerScript.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        breakPanel.SetActive(false);
        startPromptText.text = "";

        foreach (var spawner in zombieSpawners)
        {
            spawner.currentZombiesAlive = 0;
        }
    }

    public void StartNextWaveFromUI()
    {
        breakPanel.SetActive(false);
        currentWave++;
        StartWave();
    }

    public void EnterPreWaveState()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        breakPanel.SetActive(true);
        startPromptText.text = $"Press F to start Wave {currentWave + 1}";
        waitingToStartWave = true;

        if (playerScript != null)
            playerScript.enabled = true;
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
}