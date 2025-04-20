using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PuzzleColor { Red, Blue, Orange, Green, Yellow }
public class RepairConsole : MonoBehaviour
{
    public float repairTime;
    public float decayOverTime;
    private float currentProgress = 0f;
    private bool isRepaired = false;
    private bool playerInRange;

    [Header("Final Phase")]
    public Image doorChargeBar;
    public float doorChargeTime;
    private float doorChargeProgress = 0f;
    public bool doorCharging = false;

    [Header("Terminal Type")]
    public bool isMainTerminal = false;
    public static int totalRepaired = 0;
    public static int totalRequired = 3;
    public static RepairConsole mainTerminalRef;

    [Header("Main terminal")]
    public List<PuzzleColor> currentPattern = new List<PuzzleColor>();
    public int patternLength = 5;

    [Header("Main Terminal Visuals")]
    public Renderer mainTerminalScreen;
    public float flashDuration;
    public float betweenFlashDelay;
    public float pauseBeforeInput;
    public float loopDelay;

    private bool isFlashing = false;

    private Dictionary<PuzzleColor, Color> colorMap = new Dictionary<PuzzleColor, Color>
{
    { PuzzleColor.Red, Color.red },
    { PuzzleColor.Blue, Color.blue },
    { PuzzleColor.Orange, new Color(1f, 0.5f, 0f) },
    { PuzzleColor.Green, Color.green },
    { PuzzleColor.Yellow, Color.yellow }
};

    [Header("UI")]
    public Image repairBar;

    [Header("Console Visuals")]
    public Renderer screenRenderer; // the background screen we want to flash
    public GameObject RepairCanvas;
    public Color flashColor = Color.red;
    public Color repairedColor = Color.green;

    [Header("Final Step")]
    public GameObject finalDoor;

    private Coroutine flashRoutine;
    private List<PuzzleColor> playerInput = new List<PuzzleColor>();
    private int inputIndex = 0;

    void Start()
    {
        currentProgress = 0f;
        if (doorChargeBar != null)
            doorChargeBar.fillAmount = 0f;

        if (mainTerminalScreen != null)
            mainTerminalScreen.material.color = Color.black;

        if (isMainTerminal)
        {
            mainTerminalRef = this;
        }
    }

    void Update()
    {
        if (doorCharging)
        {
            doorChargeProgress += Time.deltaTime;
            if (doorChargeBar != null)
                doorChargeBar.fillAmount = doorChargeProgress / doorChargeTime;

            if (doorChargeProgress >= doorChargeTime)
            {
                doorCharging = false;
                mainTerminalScreen.material.color = Color.green;
                Debug.Log("Door fully charged! You win / open door.");

                if (finalDoor != null)
                    Destroy(finalDoor); // play animation later
            }
        }

        if (isRepaired || isMainTerminal) return;

        if (playerInRange && Input.GetKey(KeyCode.E))
        {
            currentProgress += Time.deltaTime;
        }
        else
        {
            currentProgress -= Time.deltaTime * decayOverTime;
        }

        currentProgress = Mathf.Clamp(currentProgress, 0, repairTime);

        if (repairBar != null)
            repairBar.fillAmount = currentProgress / repairTime;

        if (currentProgress >= repairTime)
        {
            CompleteRepair();
        }
    }

    private void CompleteRepair()
    {
        if (isRepaired) return;
        isRepaired = true;
        totalRepaired++;
        Debug.Log($"{gameObject.name} repaired! Total repaired: {totalRepaired}");

        if (repairBar != null)
            repairBar.color = repairedColor;

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        if (screenRenderer != null)
            screenRenderer.material.color = repairedColor;

        if (RepairCanvas != null)
            RepairCanvas.SetActive(false);

        if (totalRepaired >= totalRequired && mainTerminalRef != null && !mainTerminalRef.isFlashing)
        {
            mainTerminalRef.TriggerMainTerminal();
        }
    }

    public void TriggerMainTerminal()
    {
        if (isMainTerminal && !isFlashing)
        {
            isRepaired = true;
            GenerateNewPattern();
            StartCoroutine(PlayPatternSequence());
        }
    }


    public void SubmitColor(PuzzleColor inputColor)
    {
        if (isFlashing || !isRepaired || inputIndex >= currentPattern.Count) return;


        Debug.Log($"Player pressed: {inputColor}");

        if (currentPattern[inputIndex] == inputColor)
        {
            inputIndex++;
            if (inputIndex >= currentPattern.Count)
            {
                Debug.Log("Pattern matched successfully!");
                doorCharging = true;
                doorChargeProgress = 0f;
                if (doorChargeBar != null)
                    doorChargeBar.gameObject.SetActive(true);

                enemyAI[] allZombies = FindObjectsOfType<enemyAI>();
                foreach (enemyAI zombie in allZombies)
                {
                    zombie.SetTargetToTerminal(transform);
                }
            }
        }
        else
        {
            Debug.Log("Incorrect input! Resetting pattern.");
            GenerateNewPattern();
            inputIndex = 0;
            StartCoroutine(PlayPatternSequence());
        }
    }
    private void GenerateNewPattern()
    {
        inputIndex = 0;
        currentPattern.Clear();

        if (patternLength <= 0)
        {
            Debug.LogWarning("Pattern length is 0 or less.");
            return;
        }

        for (int i = 0; i < patternLength; i++)
        {
            PuzzleColor randomColor = (PuzzleColor)Random.Range(0, System.Enum.GetValues(typeof(PuzzleColor)).Length);
            currentPattern.Add(randomColor);
        }

        Debug.Log("Generated Pattern: " + string.Join(", ", currentPattern));
    }

    public void ApplyTerminalDamage(float damageAmount)
    {
        if (!doorCharging) return;

        doorChargeProgress -= damageAmount;
        doorChargeProgress = Mathf.Clamp(doorChargeProgress, 0f, doorChargeTime);

        if (doorChargeBar != null)
            doorChargeBar.fillAmount = doorChargeProgress / doorChargeTime;

        Debug.Log($"[Terminal] Damaged by zombie. Progress: {doorChargeProgress}/{doorChargeTime}");
    }

    private IEnumerator PlayPatternSequence()
    {
        isFlashing = true;

        foreach (var color in currentPattern)
        {
            FlashColor(colorMap[color]);
            yield return new WaitForSeconds(flashDuration);

            ClearScreen();
            yield return new WaitForSeconds(betweenFlashDelay);
        }

        yield return new WaitForSeconds(pauseBeforeInput);
        isFlashing = false;

        Debug.Log("Ready for player input!");
    }

    private IEnumerator FlashScreen()
    {
        float duration = 1f;
        Material mat = screenRenderer.material;
        Color originalColor = mat.color;

        while (!isRepaired)
        {
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                float lerp = Mathf.PingPong(t, duration) / duration;
                mat.color = Color.Lerp(originalColor, flashColor, lerp);
                yield return null;
            }
        }

        mat.color = originalColor;
    }
    private void FlashColor(Color color)
    {
        if (mainTerminalScreen != null)
            mainTerminalScreen.material.color = color;
    }

    private void ClearScreen()
    {
        if (mainTerminalScreen != null)
            mainTerminalScreen.material.color = Color.black;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;

        if (!isRepaired && flashRoutine == null)
            flashRoutine = StartCoroutine(FlashScreen());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}