
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

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

    public bool isPaused;
    private int goalCount;

    void Awake()
    {
        Application.targetFrameRate = frameRate;
        instance = this;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        weaponNotification = FindAnyObjectByType<WeaponNotificationUI>();

        waveInfoText.text = "";
        breakPanel.SetActive(false);

        ShowPreWavePrompt();
    }

    void Update()
    {
        HandlePauseInput();
        HandleWaveLogic();
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
            UpdateWaveInfoText($"Wave {currentWave + 1} | Time: {Mathf.CeilToInt(waveTimer)}s");

            if (waveTimer <= 0)
            {
                waveTimer = 0;
                waveActive = false;
                UpdateAliveCounterUI();
            }

            foreach (var spawner in zombieSpawners)
            {
                spawner.SpawnOverTime();
            }
        }
        else if (!waitingToStartWave && GetTotalZombiesAlive() <= 0)
        {
            currentWave++;
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
        waveTimer = waveDuration;
        waveActive = true;
        waitingToStartWave = false;

        breakPanel.SetActive(false);
        startPromptText.text = "";

        foreach (var spawner in zombieSpawners)
        {
            spawner.currentZombiesAlive = 0;
        }

        ResumeGame();
    }

    private void ShowPreWavePrompt()
    {
        breakPanel.SetActive(true);
        startPromptText.text = $"Press F to start Wave {currentWave + 1}";
        waitingToStartWave = true;
        EventSystem.current.SetSelectedGameObject(menuPause.transform.GetChild(0).gameObject);
    }

    public void updateGameGoal(int amount)
    {
        goalCount += amount;
        if (goalCount <= 0)
        {
            PauseGame();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void youLose()
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
        UpdateWaveInfoText($"Wave {currentWave + 1} | Cleanup Phase | Alive: {GetTotalZombiesAlive()}");
    }
}