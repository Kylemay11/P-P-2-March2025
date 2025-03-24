using UnityEngine;

public class OpenShop : MonoBehaviour
{
    private GameObject shop;
    private GameObject Perkshop;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shop = GameObject.Find("ShopUI");
        if (shop != null)
        {
            shop.SetActive(false);
        }

        Perkshop = GameObject.Find("PerkShopUI");
        if (Perkshop != null)
        {
            Perkshop.SetActive(false);
        }


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

        if (Perkshop != null)
        {
            if (other.CompareTag("Player"))
            {
                Perkshop.SetActive(true);
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
        if (Perkshop != null)
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
