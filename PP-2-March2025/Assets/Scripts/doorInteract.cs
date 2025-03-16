using UnityEngine;
using TMPro;

public class DoorInteract : MonoBehaviour
{
    [SerializeField]public int doorCost;
   [SerializeField] public GameObject doorVisual; // The door model (optional if you just disable it)
   [SerializeField] public GameObject roomToActivate; // Room or spawners to enable

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
            Destroy(doorVisual); // Or play animation

        if (roomToActivate != null)
            roomToActivate.SetActive(true);

        if (interactionUI != null)
            interactionUI.SetActive(false);

        Debug.Log("Door Unlocked!");
    }

    void UpdateUIText()
    {
        TextMeshProUGUI txt = interactionUI.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            txt.text = $"Press E to open (${doorCost})";
        }
    }
}