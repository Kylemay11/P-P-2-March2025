using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static enemyAI;

public class BarricadeDoor : MonoBehaviour
{

    [Header("Door Settings")]
    [SerializeField] private int maxPlanks = 6;
    [SerializeField] private float totalDoorHealth;
    [SerializeField] private float repairDuration;
    [SerializeField] private int repairCostPerPlank;
    [SerializeField] private int maxAttackers;

    [Header("References")]
    [SerializeField] private List<GameObject> planks; // Assign plank objects in order from top to bottom
    [SerializeField] private GameObject repairPromptUI;
    [SerializeField] private TMPro.TextMeshProUGUI repairPromptText;
    [SerializeField] private TextMeshProUGUI repairCostText;
    [SerializeField] private List<Transform> doorAttackPoints;
    [SerializeField] private bool isTrainingDoor = false;

    private float currentHealth;
    private int planksRemaining;
    private bool isPlayerNear;
    private bool isRepairing;
    private float heldDeration;
    [SerializeField] private List<enemyAI> activeAttackers = new List<enemyAI>();

    public enum DoorState { Intact, Damaged, Destroyed }
    public DoorState CurrentState { get; private set; } = DoorState.Intact;

    private void Start()
    {
        currentHealth = totalDoorHealth;
        planksRemaining = maxPlanks;
        UpdatePlankVisuals();

        if (isTrainingDoor)
        {
            ApplyDamage(50); // Reduce health and planks at game start
        }
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

        int originalPlanks = planksRemaining;

        while (heldDeration < repairDuration && Input.GetButton("Interact"))
        {
            if (planksRemaining == maxPlanks)
                break;

            heldDeration += Time.deltaTime;

            // Live repair progress as percentage
            float repairProgress = Mathf.Clamp01(heldDeration / repairDuration);
            int estimatedPlanks = Mathf.FloorToInt(repairProgress * maxPlanks);
            int desiredPlanks = Mathf.Min(maxPlanks, originalPlanks + estimatedPlanks);

            if (desiredPlanks > planksRemaining)
            {
                int planksAdded = desiredPlanks - planksRemaining;
                planksRemaining = desiredPlanks;

                // Sync health with visual planks
                currentHealth = (planksRemaining / (float)maxPlanks) * totalDoorHealth;

                UpdatePlankVisuals();
            }

            int dynamicCost = (maxPlanks - planksRemaining) * repairCostPerPlank;

            if (repairCostText != null)
                repairCostText.text = $"Repair Cost: ${dynamicCost}";

            yield return null;
        }

        // Finalize repair cost and confirm payment
        int finalPlanksToRestore = planksRemaining - originalPlanks;
        int finalCost = finalPlanksToRestore * repairCostPerPlank;

        if (CurrencySystem.instance.SpendMoney(finalCost))
        {
            currentHealth = Mathf.Min(currentHealth, totalDoorHealth);
            CurrentState = (planksRemaining == maxPlanks) ? DoorState.Intact : DoorState.Damaged;

            if (CurrentState != DoorState.Destroyed)
                NotifyZombiesDoorRepaired();
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
        foreach (var col in GetComponents<Collider>())
        {
            if (!col.isTrigger)
                col.enabled = true;
        }

        enemyAI[] Zombie = FindObjectsByType<enemyAI>(FindObjectsSortMode.None);
        foreach (var enemy in Zombie)
        {
            if (enemy != null && enemy.barrierDoor == this)
            {
                enemy.SetTargetState(ZombieTargetState.AttackingPlayer);
            }
        }
    }

    private void NotifyZombiesDoorRepaired()
    {
        foreach (var col in GetComponents<Collider>())
        {
            if (!col.isTrigger)
                col.enabled = false;
        }

        enemyAI[] Zombie = FindObjectsByType<enemyAI>(FindObjectsSortMode.None);
        foreach (var enemy in Zombie)
        {
            if (enemy != null && enemy.barrierDoor == this)
            {
                enemy.SetTargetState(ZombieTargetState.AttackingDoor);
            }
        }
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

            if (isTrainingDoor)
            {
                ApplyDamage(50);
            }
        }
    }
    public Transform GetRandomAttackPoint()
    {
        if (doorAttackPoints == null || doorAttackPoints.Count == 0)
            return transform; 

        return doorAttackPoints[Random.Range(0, doorAttackPoints.Count)];
    }

    public bool CanZombieAttack(enemyAI Zombie)
    {
        if (!activeAttackers.Contains(Zombie) && activeAttackers.Count >= maxAttackers)
            return false;

        if (!activeAttackers.Contains(Zombie))
            activeAttackers.Add(Zombie);
        return true;
    }

    public void RemoveAttacker(enemyAI Zombie)
    {
        if (activeAttackers.Contains(Zombie))
            activeAttackers.Remove(Zombie);
    }
}