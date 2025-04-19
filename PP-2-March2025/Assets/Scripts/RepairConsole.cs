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

    [Header("Main terminal")]
    public List<PuzzleColor> currentPattern = new List<PuzzleColor>();
    public int patternLength = 5;

    [Header("Main Terminal Visuals")]
    public Renderer mainTerminalScreen;
    public float flashDuration = 0.5f;
    public float betweenFlashDelay = 0.3f;
    public float pauseBeforeInput = 1f;

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

    private Coroutine flashRoutine;
    private List<PuzzleColor> playerInput = new List<PuzzleColor>();
    private int inputIndex = 0;

    void Start()
    {
        currentProgress = 0f;
    }

    void Update()
    {
        if (isRepaired) return;

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

        if (currentProgress == repairTime)
        {
            CompleteRepair();
        }
    }

    private void CompleteRepair()
    {
        isRepaired = true;
        Debug.Log($"{gameObject.name} repaired!");

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

        GenerateNewPattern();
        StartCoroutine(PlayPatternSequence());

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
                // Add your puzzle completion logic here
            }
        }
        else
        {
            Debug.Log("Incorrect input! Resetting...");
            GenerateNewPattern();
            inputIndex = 0;
            StartCoroutine(PlayPatternSequence());
        }
    }

    private void GenerateNewPattern()
    {
        currentPattern.Clear();
        for (int i = 0; i < patternLength; i++)
        {
            PuzzleColor randomColor = (PuzzleColor)Random.Range(0, System.Enum.GetValues(typeof(PuzzleColor)).Length);
            currentPattern.Add(randomColor);
        }

        Debug.Log("Generated Pattern: " + string.Join(", ", currentPattern));
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