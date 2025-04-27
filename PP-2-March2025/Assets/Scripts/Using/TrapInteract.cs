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

    private TrapDamage trapDamage;
    private bool isActive = false;
    private bool isPlayerNear = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip purchaseSound;
    [Range(0, 1)][SerializeField] private float activateSoundVolume;
    [SerializeField] private AudioClip sawSound;
    [SerializeField] private AudioClip spikeSound;
    [Range(0, 1)][SerializeField] private float trapSoundVolume;

    public enum trapType
    {
        Saw,
        Spike
    }

    [Header("Trap Type")]
    [SerializeField] private trapType trap;
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

        if (Input.GetButtonDown("Interact"))
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
        activateTrapSound();
        playTrapSound();
        Invoke("DeactivateTrap", activeDuration);
    }

    private void DeactivateTrap()
    {
        isActive = false;
        SetTrapState(false);
        stopTrapSound();
    }
    private void stopTrapSound()
    {
        if(audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }
    private void SetTrapState(bool active)
    {
        if (inactiveVisual) inactiveVisual.SetActive(!active);
        if (activeVisual) activeVisual.SetActive(active);
        if (trapDamage) trapDamage.SetActive(active);
        if (interactionUI) interactionUI.SetActive(false);
    }

    private void activateTrapSound()
    {
        if (audioSource != null && purchaseSound != null)
        {
            audioSource.PlayOneShot(purchaseSound, activateSoundVolume);
        }
        
    }

    private void playTrapSound()
    {
        if(trap == trapType.Saw)
        {
            if (audioSource != null && sawSound != null)
            {
                audioSource.clip = sawSound;
                audioSource.loop = true;
                audioSource.volume = trapSoundVolume;
                audioSource.Play();
            }
        }
        else if(trap == trapType.Spike)
        {
            if (audioSource != null && spikeSound != null)
            {
                audioSource.clip = spikeSound;
                audioSource.loop = true;
                audioSource.volume = trapSoundVolume;
                audioSource.Play();
            }
        }

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