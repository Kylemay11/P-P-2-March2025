using UnityEngine;


public class OpenPerkShop : MonoBehaviour
{
    public static OpenPerkShop Instance;
    private GameObject Perkshop;
    public bool isPlayerNear = false;
    public bool isInShop = false;
    [Header("--- Audio ---")]
    [SerializeField] private AudioSource aud;
    [SerializeField] private AudioClip audShopOpen;
    [Range(0, 1)][SerializeField] private float audShopOpenVol = 1f;

    
    void Start()
    {
        Instance = this;
        Perkshop = GameObject.Find("PerkShopUI");
        if (Perkshop != null)
        {
            Perkshop.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerNear && Input.GetButtonDown("Interact"))
        {
            TogglePerkShop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            
            untogglePerkShop();
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void TogglePerkShop()
    {
        isInShop = true;
        gameManager.instance.menuPerkShop.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerController.instance.canMove = false;
        cameraComtroller.instance.canLook = false;
        if (aud != null && audShopOpen != null)
        {
            aud.PlayOneShot(audShopOpen, audShopOpenVol);
        }
    }

    public void untogglePerkShop()
    {
        isInShop = false;
        gameManager.instance.menuPerkShop.SetActive(false);
        playerController.instance.canMove = true;
        cameraComtroller.instance.canLook = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}