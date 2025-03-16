using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarricadeDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private int maxPlanks = 6;
    [SerializeField] private float totalDoorHealth;
    [SerializeField] private float repairDuration;
    [SerializeField] private int repairCostPerPlank;

    [Header("References")]
    [SerializeField] private List<GameObject> planks; // Assign plank objects in order from top to bottom
    [SerializeField] private GameObject repairPromptUI;
    [SerializeField] private TMPro.TextMeshProUGUI repairPromptText;
    [SerializeField] private TextMeshProUGUI repairCostText;

    private float currentHealth;
    private int planksRemaining;
    private bool isPlayerNear;
    private bool isRepairing;
    private float heldDeration;

    public enum DoorState { Intact, Damaged, Destroyed }
    public DoorState CurrentState { get; private set; } = DoorState.Intact;

    private void Start()
    {
        currentHealth = totalDoorHealth;
        planksRemaining = maxPlanks;
        UpdatePlankVisuals();

        ApplyDamage(50);
    }

    private void Update()
    {
        if (isPlayerNear && CurrentState != DoorState.Intact && !isRepairing)
        {
            if (Input.GetButton("Interact") && !isRepairing)
            {
                StartCoroutine(RepairDoor());
            }
        }
    }

    public void ApplyDamage(float amount)
    {
        if (CurrentState == DoorState.Destroyed) return;

        currentHealth -= amount;

        int newPlankCount = Mathf.CeilToInt((currentHealth / totalDoorHealth) * maxPlanks);
        if (newPlankCount < planksRemaining)
        {
            planksRemaining = newPlankCount;
            UpdatePlankVisuals();
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            CurrentState = DoorState.Destroyed;
            UpdatePlankVisuals();
            NotifyZombiesDoorBroken();
        }
        else
        {
            CurrentState = DoorState.Damaged;
        }
    }

    private IEnumerator RepairDoor()
    {
        isRepairing = true;
        heldDeration = 0f;

        if (repairPromptText != null)
            repairPromptText.text = "Repairing...";

        while (heldDeration < repairDuration && Input.GetButton("Interact"))
        {
            if (planksRemaining == maxPlanks)
                break;
            heldDeration += Time.deltaTime;

            // Live repair progress
            float repairProgress = Mathf.Clamp01(heldDeration / repairDuration);
            int estimatedPlanks = Mathf.FloorToInt(repairProgress * maxPlanks);
            int planksToRestore = Mathf.Min(maxPlanks - planksRemaining, estimatedPlanks);
            int dynamicCost = planksToRestore * repairCostPerPlank;

            if (repairCostText != null)
                repairCostText.text = $"Repair Cost: ${dynamicCost}";

            yield return null;
        }

        float finalRepairProgress = Mathf.Clamp01(heldDeration / repairDuration);
        float restoredHealth = finalRepairProgress * totalDoorHealth;
        int restoredPlanks = Mathf.FloorToInt(finalRepairProgress * maxPlanks);
        int finalPlanksToRestore = Mathf.Min(maxPlanks - planksRemaining, restoredPlanks);
        int finalCost = finalPlanksToRestore * repairCostPerPlank;

        if (CurrencySystem.instance.SpendMoney(finalCost))
        {
            currentHealth += restoredHealth;
            currentHealth = Mathf.Min(currentHealth, totalDoorHealth);
            planksRemaining += finalPlanksToRestore;
            planksRemaining = Mathf.Min(planksRemaining, maxPlanks);
            UpdatePlankVisuals();
            CurrentState = (planksRemaining == maxPlanks) ? DoorState.Intact : DoorState.Damaged;
        }

        // Reset UI
        if (repairPromptText != null)
            repairPromptText.text = $"";
        if (repairCostText != null)
            repairCostText.text = "";

        if (planksRemaining == maxPlanks && repairPromptUI != null)
            repairPromptUI.SetActive(false);

        isRepairing = false;
    }

    private void UpdatePlankVisuals()
    {
        for (int i = 0; i < planks.Count; i++)
        {
            planks[i].SetActive(i < planksRemaining);
        }
    }

    private void UpdateRepairCostText()
    {
        if (repairPromptText != null && CurrentState != DoorState.Intact)
            repairPromptText.text = "Hold [E] to Repair";

        if (repairCostText != null && CurrentState != DoorState.Intact)
        {
            int missingPlanks = maxPlanks - planksRemaining;
            int cost = missingPlanks * repairCostPerPlank;
            repairCostText.text = $"(${cost})";
        }
    }

    private void NotifyZombiesDoorBroken()
    {
        // TODO: Implement zombie AI to check if the door is broken and retarget to the player.
        // You could trigger a door event or change aggro state.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;

            if (repairPromptUI != null && CurrentState != DoorState.Intact)
                repairPromptUI.SetActive(true);

            UpdateRepairCostText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (repairPromptUI != null)
                repairPromptUI.SetActive(false);
        }
    }
}
