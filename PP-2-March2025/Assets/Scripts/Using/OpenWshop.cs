using UnityEngine;

public class OpenWshop : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static OpenShop instance;
    private GameObject shop2;
    public bool isPlayerNear = false;

    [Header("--- Audio ---")]
    [SerializeField] private AudioSource aud;
    [SerializeField] private AudioClip audShopOpen;
    [Range(0, 1)][SerializeField] private float audShopOpenVol = 1f;

    void Start()
    {
        shop2 = GameObject.Find("WShopUI");
        if (shop2 != null)
        {
            shop2.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerNear && Input.GetButtonDown("Interact"))
        {
            ToggleWShop();
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
            UnToggleWShop();
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleWShop()
    {

        gameManager.instance.menuWeapShop.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerController.instance.canMove = false;
        cameraComtroller.instance.canLook = false;
        if (aud != null && audShopOpen != null)
        {
            aud.PlayOneShot(audShopOpen, audShopOpenVol);
        }

    }

    public void UnToggleWShop()
    {
        gameManager.instance.menuWeapShop.SetActive(false);
        playerController.instance.canMove = true;
        cameraComtroller.instance.canLook = true;
    }
}