using UnityEngine;
using TMPro;
using UnityEngine.AI;
using System.Collections;

public class DoorInteract : MonoBehaviour
{
    [Header("Door Type")]
    [SerializeField] private DoorType doorType = DoorType.BuyDoor;

    [SerializeField] private bool eventComplete = false;

    [Header("Door Setup")]
    [SerializeField] private int doorCost;
    [SerializeField] private GameObject doorVisual; // Optional door model to hide
    [SerializeField] private GameObject roomToActivate; // Room prefab that holds RoomSpawnerManager

    [Header("UI")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI doorLockedText;


    private bool isUnlocked = false;
    public bool isPlayerNear = false;
    private bool hasMarkedChecklist = false;

    private RoomSpawnerManager roomSpawnerManager;

    public enum DoorType { BuyDoor, EventDoor }

    void Start()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
            UpdateUIText();
        }

        if (doorLockedText != null)
            doorLockedText.gameObject.SetActive(false);

        if (roomToActivate != null)
        {
            roomSpawnerManager = roomToActivate.GetComponent<RoomSpawnerManager>();
            if (roomSpawnerManager == null)
                Debug.LogWarning($"{gameObject.name} door assigned a room without RoomSpawnerManager.");
        }
    }

    void Update()
    {
        if (!isPlayerNear || isUnlocked)
            return;

        if (!Input.GetButtonDown("Interact"))
            return;

        if (doorType == DoorType.BuyDoor)
        {
            TryUnlock();
        }
        else if (doorType == DoorType.EventDoor)
        {
            if (eventComplete)
            {
                UnlockDoor();
            }
            else
            {
                Debug.Log("Event door not ready yet!");
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

        if (!hasMarkedChecklist)
        {
            if (gameObject.name == "FiringRangeDoor")
                FindObjectOfType<TutorialChecklistUI>().CompleteObjective(1);
            else if (gameObject.name == "FinalTutorialDoor")
                FindObjectOfType<TutorialChecklistUI>().CompleteObjective(4);

            hasMarkedChecklist = true;
        }
    }

    void UpdateUIText()
    {
        TextMeshProUGUI txt = interactionUI.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            if (doorType == DoorType.BuyDoor)
                txt.text = $"Press [E] to open (${doorCost})";
            else if (doorType == DoorType.EventDoor)
                txt.text = eventComplete ? "Press [E] to open (Event Complete)" : "Door Locked: Event Incomplete";
        }
    }
    public void CompleteEvent()
    {
        eventComplete = true;
        UpdateUIText();
    }
    public bool IsUnlocked()
    {
        return isUnlocked;
    }


    private IEnumerator ShowDoorLockedMessage()
    {
        doorLockedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        doorLockedText.gameObject.SetActive(false);
    }

}