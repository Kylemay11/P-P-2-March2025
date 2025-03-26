using UnityEngine;


public class OpenShop : MonoBehaviour
{
    private GameObject shop;
    private bool isActive = false;
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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void ToggleShop()
    {
        isActive = !isActive;
        if (shop != null)
            shop.SetActive(isActive);
    }
}