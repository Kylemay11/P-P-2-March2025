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

    public bool isPaused;

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
    }

    // Update is called once per frame
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

            if (waveTimer <= 0)
            {
                waveTimer = 0;
                waveActive = false;
                Debug.Log("Wave timer ended! Waiting for all zombies to be cleared...");
            }

            // Spawn zombies while timer is active
            zombieSpawner.SpawnOverTime();
        }
        else
        {
            // Wait for wave cleanup: no zombies left alive
            if (zombieSpawner.currentZombiesAlive <= 0)
            {
                Debug.Log("Wave Complete!");
                currentWave++;
                StartWave(); // Start next wave
            }
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
        waveTimer = waveDuration;
        waveActive = true;

        Debug.Log("Wave " + (currentWave + 1) + " started!");

        zombieSpawner.maxZombies = 10 + currentWave * 5;
        zombieSpawner.spawnRate = 2f;

        zombieSpawner.currentZombiesAlive = 0;
    }
}