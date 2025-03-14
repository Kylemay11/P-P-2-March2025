using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] public ZombieSpawner zombieSpawner;

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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !waitingToStartWave)
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
        if (waitingToStartWave && playerScript != null && !playerScript.enabled)
        {
            playerHPBar.fillAmount = (float)playerScript.currentHP / playerScript.maxHP;
            playerStaminaBar.fillAmount = 1f;
        }

        if (waveActive)
        {
            waveTimer -= Time.deltaTime;
            waveInfoText.text = $"Wave {currentWave + 1} | Time: {Mathf.CeilToInt(waveTimer)}s";

            if (waveTimer <= 0)
            {
                waveTimer = 0;
                waveActive = false;
                waveInfoText.text = $"Wave {currentWave + 1} | Cleanup Phase | Alive: {zombieSpawner.currentZombiesAlive}";
                Debug.Log("Wave timer ended! Waiting for all zombies to be cleared...");
            }

            zombieSpawner.SpawnOverTime();
        }
        else if (!waitingToStartWave)
        {
            if (zombieSpawner.currentZombiesAlive <= 0)
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
        playerScript.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpaused()
    {
        isPaused = !isPaused;
        playerScript.enabled = true;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
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
        Time.timeScale = 1f;
        waveTimer = waveDuration;
        waveActive = true;
        waitingToStartWave = false;

        Debug.Log("Wave " + (currentWave + 1) + " started!");

        breakPanel.SetActive(false);
        startPromptText.text = "";
        zombieSpawner.currentZombiesAlive = 0;
    }

    public void StartNextWaveFromUI()
    {
        waitingToStartWave = false;
        StartWave();
    }

    public void EnterPreWaveState()
    {
        isPaused = false;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        breakPanel.SetActive(true);
        startPromptText.text = $"Press F to start Wave {currentWave + 1}";
        waitingToStartWave = true;
    }
}