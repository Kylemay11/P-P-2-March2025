using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class TrapInteract : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private int activationCost;
    [SerializeField] private GameObject inactiveVisual;
    [SerializeField] private GameObject activeVisual;
    [Range(1, 20)] [SerializeField] private float activeDuration;

    [Header("UI")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private AudioClip activateSound;

    private TrapDamage trapDamage;
    private AudioSource audioSource;
    private bool isActive = false;
    private bool isPlayerNear = false;

    void Start()
    {
        // Initialize all local components
        trapDamage = GetComponentInChildren<TrapDamage>(true);
        audioSource = GetComponent<AudioSource>();

        // Explicitly reset state
        if (inactiveVisual) inactiveVisual.SetActive(true);
        if (activeVisual) activeVisual.SetActive(false);
        trapDamage.SetActive(false);
    }

    void Update()
    {
        if (!isPlayerNear || isActive) return;

        if (Input.GetButtonDown("Interact") && !gameManager.instance.waveActive)
            TryActivate();
    }

    private void TryActivate()
    {
        if (CurrencySystem.instance.SpendMoney(activationCost))
            ActivateTrap();
    }

    private void ActivateTrap()
    {
        isActive = true;
        SetTrapState(true);
        PlaySound(activateSound);
        Invoke("DeactivateTrap", activeDuration);
    }

    private void DeactivateTrap()
    {
        isActive = false;
        SetTrapState(false);
    }

    private void SetTrapState(bool active)
    {
        if (inactiveVisual) inactiveVisual.SetActive(!active);
        if (activeVisual) activeVisual.SetActive(active);
        if (trapDamage) trapDamage.SetActive(active);
        if (interactionUI) interactionUI.SetActive(false);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            isPlayerNear = true;
            if (interactionUI)
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
            if (interactionUI)
            {
                interactionUI.SetActive(false);
                ClearUIText();
            }
        }
    }

    void UpdateUIText()
    {
        if (costText != null)
            costText.text = $"Press E to activate.(${activationCost})";
    }

    void ClearUIText()
    {
        if (costText != null)
            costText.text = "";
    }
}