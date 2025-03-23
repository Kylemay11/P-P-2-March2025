using UnityEngine;
using TMPro;
using UnityEngine.AI;
using NUnit.Framework;
using System.Collections.Generic;

public class DoorInteract : MonoBehaviour
{
    [SerializeField] public int doorCost;
    [SerializeField] public GameObject doorVisual; // The door model (optional if you just disable it)
    [SerializeField] public GameObject roomToActivate; // Room or spawners to enable
    [SerializeField] private List<ZombieSpawner> spawnersToActivate;

    public GameObject interactionUI;

    private bool isUnlocked = false;
    private bool isPlayerNear = false;

    void Start()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
            UpdateUIText();
        }
    }

    void Update()
    {
        if (isPlayerNear && !isUnlocked)
        {
            if (Input.GetButtonDown("Interact"))
            {
                TryUnlock();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isUnlocked)
        {
            isPlayerNear = true;

            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
                UpdateUIText();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;

            if (interactionUI != null)
                interactionUI.SetActive(false);
        }
    }

    void TryUnlock()
    {
        if (CurrencySystem.instance.SpendMoney(doorCost))
        {
            UnlockDoor();
        }
        else
        {
            Debug.Log("Not enough money to open door.");
        }
    }

    void UnlockDoor()
    {
        isUnlocked = true;

        if (doorVisual != null)
            doorVisual.SetActive(false); // Or play animation

        NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();
        if (obstacle != null)
        {
            obstacle.carving = false;
            obstacle.enabled = false;
        }

        // Optional auto-room activation
        if (roomToActivate != null)
        {
            roomToActivate.SetActive(true);

            ZombieSpawner[] roomSpawners = roomToActivate.GetComponentsInChildren<ZombieSpawner>();
            foreach (var spawner in roomSpawners)
            {
                spawner.SetSpawnerActive(true);
            }
        }

        // Manual spawner assignment (optional override or extra)
        if (spawnersToActivate != null && spawnersToActivate.Count > 0)
        {
            foreach (var spawner in spawnersToActivate)
            {
                if (spawner != null)
                {
                    spawner.SetSpawnerActive(true);
                }
            }
        }

        if (interactionUI != null)
            interactionUI.SetActive(false);

        Debug.Log("Door Unlocked!");
    }

    void UpdateUIText()
    {
        TextMeshProUGUI txt = interactionUI.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            txt.text = $"Press [E] to open (${doorCost})";
        }
    }

}