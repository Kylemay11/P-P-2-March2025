using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class DoorInteract : MonoBehaviour
{
    [Header("Door Setup")]
    [SerializeField] private int doorCost;
    [SerializeField] private GameObject doorVisual; // Optional door model to hide
    [SerializeField] private GameObject roomToActivate; // Room prefab that holds RoomSpawnerManager

    [Header("UI")]
    [SerializeField] private GameObject interactionUI;

    private bool isUnlocked = false;
    private bool isPlayerNear = false;

    private RoomSpawnerManager roomSpawnerManager;

    void Start()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
            UpdateUIText();
        }

        if (roomToActivate != null)
        {
            roomSpawnerManager = roomToActivate.GetComponent<RoomSpawnerManager>();
            if (roomSpawnerManager == null)
                Debug.LogWarning($"{gameObject.name} door assigned a room without RoomSpawnerManager.");
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


    private void UnlockDoor()
    {
        isUnlocked = true;

        // Hide door model
        if (doorVisual != null)
            doorVisual.SetActive(false);

        // Disable NavMeshObstacle
        NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();
        if (obstacle != null)
        {
            obstacle.carving = false;
            obstacle.enabled = false;
        }

        // Activate the room GameObject
        if (roomToActivate != null)
            roomToActivate.SetActive(true);

        // Tell room spawner manager that this room is now active
        if (roomSpawnerManager != null)
        {
            foreach (var spawner in roomToActivate.GetComponentsInChildren<ZombieSpawner>())
            {
                spawner.SetSpawnerActive(true);
            }

            roomSpawnerManager.BuildRoomPhases();
        }


        // Hide interaction UI
        if (interactionUI != null)
            interactionUI.SetActive(false);

        Debug.Log($"{gameObject.name} Door Unlocked!");
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