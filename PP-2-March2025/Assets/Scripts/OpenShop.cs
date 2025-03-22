using UnityEngine;

public class OpenShop : MonoBehaviour
{
    private GameObject shop;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (shop != null)
        {
            shop.SetActive(false);
        }
        shop = GameObject.Find("ShopUI");
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (shop != null)
        {
            if (other.CompareTag("Player"))
            {
                shop.SetActive(true);
            }
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    void OnTriggerExit(Collider other)
    {
        if (shop != null)
        {
            if (other.CompareTag("Player"))
            {
                shop.SetActive(false);
            }
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
