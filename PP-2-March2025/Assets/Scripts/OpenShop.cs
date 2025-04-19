using UnityEngine;


public class OpenShop : MonoBehaviour
{
    public static OpenShop instance;
    private GameObject shop;
    public bool isPlayerNear = false;

    void Start()
    {
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
        gameManager.instance.menuShop.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //gameManager.instance.PauseGame();
    }

    public void UnToggleShop()
    {
        gameManager.instance.menuShop.SetActive(false);
        //gameManager.instance.ResumeGame();
    }
}