using UnityEngine;


public class OpenShop : MonoBehaviour
{
    public static OpenShop instance;
    private GameObject shop;
   
    public bool isPlayerNear = false;
    public bool isInShop = false;
    [Header("--- Audio ---")]
    [SerializeField] private AudioSource aud;
    [SerializeField] private AudioClip audShopOpen;
    [Range(0, 1)][SerializeField] private float audShopOpenVol = 1f;

    void Start()
    {
        instance = this;
        shop = GameObject.Find("ShopUI");
        if (shop != null)
        {
            shop.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerNear && Input.GetButtonDown("Interact"))
        {
            ToggleShop();
           
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
            UnToggleShop();
           
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleShop()
    {
        isInShop = true;
        gameManager.instance.menuShop.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerController.instance.canMove = false;
        cameraComtroller.instance.canLook = false;
        if (aud != null && audShopOpen != null)
        {
            aud.PlayOneShot(audShopOpen, audShopOpenVol);
        }

    }

    public void UnToggleShop()
    {
        isInShop = false;
        gameManager.instance.menuShop.SetActive(false);
        playerController.instance.canMove = true;
        cameraComtroller.instance.canLook = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}