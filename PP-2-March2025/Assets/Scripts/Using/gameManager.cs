using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("Performance")]
    [SerializeField] private int frameRate;

    [Header("Menus")]

    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuPerkInventory;
   

    [SerializeField] public GameObject menuMain;
    [SerializeField] public GameObject menuLvlSelect;
    [SerializeField] public GameObject menuCredits;
    [SerializeField] public GameObject menufpsDisplay;
    [SerializeField] public GameObject menusettings;
    [SerializeField] public GameObject menuShop;
    [SerializeField] public GameObject menuPerkShop;

    public GameObject menuActive;

    [Header("Wave Settings")]
    [SerializeField] private float waveDuration;
    [SerializeField] private float healthMultiplier;
    [SerializeField] private float damageMultiplier;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private DoorInteract finalTutorialDoor;

    private float waveTimer;
    public bool waveActive = false;
    private bool waitingToStartWave = true;
    private int currentWave;

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

    [Header("Perk Inventory UI")]
    public Image BonusHealthIcon;
    public Image BonusSpeedIcon;
    public Image BonusStaminaIcon;
    public Image BonusGunDamageIcon;
    public Image BonusGunFireRateIcon;
    public Image BonusGunFireDistIcon;

    public bool isPaused = false;
    public bool objectiveMode = false;

    private int goalCount = 0;

    private void Awake()
    {
        Application.targetFrameRate = frameRate;
        instance = this;

        if (player == null) player = GameObject.FindWithTag("Player");
        if (player != null) playerScript = player.GetComponent<playerController>();
        if (weaponNotification == null) weaponNotification = FindAnyObjectByType<WeaponNotificationUI>();

        waveInfoText.text = string.Empty;
        breakPanel.SetActive(false);
        ShowPreWavePrompt();
    }

    private void Update()
    {
        HandlePauseInput();
        HandleWaveLogic();
        HandleInventoryInput();
    }

    private void HandlePauseInput()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                PauseGame();
                menuPause.SetActive(true);
                menuActive = menuPause;
                EventSystem.current.SetSelectedGameObject(menuPause.transform.GetChild(0).gameObject);
            }
            else if (menuActive == menuPause)
            {
                ResumeGame();
            }
        }
    }
    private void HandleInventoryInput()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if (menuActive == null)
            {
                PauseGame();
                menuPerkInventory.SetActive(true);
                menuActive = menuPerkInventory;
                EventSystem.current.SetSelectedGameObject(menuPerkInventory.transform.GetChild(0).gameObject);
            }
            else if (menuActive == menuPerkInventory)
            {
                ResumeGame();
            }
        }
    }

    private void HandleWaveLogic()
    {
        if (waveActive)
        {
            if (!objectiveMode)
                waveTimer -= Time.deltaTime;

            UpdateWaveInfoText($"Wave {currentWave} | Time: {Mathf.CeilToInt(waveTimer)}s");

            if (waveTimer <= 0)
            {
                EndWave();
            }
        }
        else if (!waitingToStartWave && GetTotalZombiesAlive() <= 0)
        {
            UpdateWaveInfoText($"Wave {currentWave} Complete!");
            ShowPreWavePrompt();
        }

        if (waitingToStartWave)
        {
            // Block starting the wave until tutorial door is open
            if (finalTutorialDoor != null && !finalTutorialDoor.IsUnlocked())
            {
                startPromptText.text = ""; // Hide the prompt
                return;
            }

            // Show prompt
            if (!startPromptText.text.Contains("Press F"))
                startPromptText.text = $"Press F to start Wave {currentWave + 1}";

            // Let player start wave
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (objectiveMode) return;
                StartWave();
            }
        }
    }

    private void EndWave()
    {
        waveTimer = 0;
        waveActive = false;

        // Stop all room waves
        RoomSpawnerManager[] roomManagers = FindObjectsByType<RoomSpawnerManager>(FindObjectsSortMode.None);
        foreach (var room in roomManagers)
        {
            room.EndRoomWave();
        }
        CleanupStragglers(25f);
        UpdateAliveCounterUI();
        StartCoroutine(WaitBeforeCleanupUI());
    }

    public void StartWave()
    {
        currentWave++;

        float healthMult = Mathf.Pow(healthMultiplier, currentWave);
        float dmgMult = Mathf.Pow(damageMultiplier, currentWave);
        float speedMult = Mathf.Pow(speedMultiplier, currentWave);

        RoomSpawnerManager[] roomManagers = FindObjectsByType<RoomSpawnerManager>(FindObjectsSortMode.None);
        foreach (var room in roomManagers)
        {
            room.InitializeRoomWave(currentWave, healthMult, dmgMult, speedMult);
        }

        waveTimer = waveDuration;
        waveActive = true;
        waitingToStartWave = false;

        breakPanel.SetActive(false);
        startPromptText.text = string.Empty;

        ResumeGame();
    }

    private void ShowPreWavePrompt()
    {
        breakPanel.SetActive(true);
        startPromptText.text = $"Press F to start Wave {currentWave + 1}";
        waitingToStartWave = true;
        EventSystem.current.SetSelectedGameObject(menuPause.transform.GetChild(0).gameObject);
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
            menusettings.SetActive(false);
            menuPerkInventory.SetActive(false);
            menuActive = null;
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void UpdateGameGoal(int amount)
    {
        goalCount += amount;
        if (goalCount <= 0)
        {
            PauseGame();
            menuWin.SetActive(true);
            menuActive = menuWin;
        }
    }

    public void YouLose()
    {
        PauseGame();
        menuLose.SetActive(true);
        menuActive = menuLose;
    }

    public void YouWin()
    {
        PauseGame();
        menuWin.SetActive(true);
        menuActive = menuWin;

        WinStatsUI statsUI = menuWin.GetComponent<WinStatsUI>();
        if (statsUI != null)
        {
            statsUI.ShowStats(
                zombiesKilled: playerScript.zombiesKilled,
                moneyEarned: CurrencySystem.instance.TotalMoney,
                wavesCleared: currentWave
            );
        }
    }
    public void CleanupStragglers(float radius)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 center = player.transform.position;
        Collider[] hits = Physics.OverlapSphere(center, radius);

        enemyAI[] allZombies = GameObject.FindObjectsByType<enemyAI>(FindObjectsSortMode.None);

        foreach (enemyAI zombie in allZombies)
        {
            bool inCleanupRadius = false;

            foreach (Collider hit in hits)
            {
                if (hit.gameObject == zombie.gameObject)
                {
                    inCleanupRadius = true;
                    break;
                }
            }

            if (!inCleanupRadius)
            {
                Debug.Log($"[Cleanup] Destroying zombie out of range: {zombie.name}");

                zombie.OnZombieDeath?.Invoke();

                Destroy(zombie.gameObject);
            }
        }
    }

    private int GetTotalZombiesAlive()
    {
        int total = 0;
        RoomSpawnerManager[] roomManagers = FindObjectsByType<RoomSpawnerManager>(FindObjectsSortMode.None);
        foreach (var room in roomManagers)
        {
            total += room.GetZombiesAlive();
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
        int count = GetTotalZombiesAlive();
        UpdateWaveInfoText($"Wave {currentWave} | Cleanup Phase | Alive: {GetTotalZombiesAlive()}");
    }

    private IEnumerator WaitBeforeCleanupUI()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateAliveCounterUI();
    }

    // perk status
    public void UnlockHealthPerk()
    {
        if (BonusHealthIcon != null)
            BonusHealthIcon.gameObject.SetActive(true);
    }
    public void UnlockSpeedPerk()
    {
        if (BonusSpeedIcon != null)
            BonusSpeedIcon.gameObject.SetActive(true);
    }
    public void UnlockStaminaPerk()
    {
        if (BonusStaminaIcon != null)
            BonusStaminaIcon.gameObject.SetActive(true);
    }
    public void UnlockGunDamagePerk()
    {
        if (BonusGunDamageIcon != null)
            BonusGunDamageIcon.gameObject.SetActive(true);
    }
    public void UnlockGunFireRatePerk()
    {
        if (BonusGunFireRateIcon != null)
            BonusGunFireRateIcon.gameObject.SetActive(true);
    }

    public void UnlockGunFireDistPerk()
    {
        if (BonusGunFireDistIcon != null)
            BonusGunFireDistIcon.gameObject.SetActive(true);
    }
}
