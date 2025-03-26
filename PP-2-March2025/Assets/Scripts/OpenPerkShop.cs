using UnityEngine;

public class OpenPerkShop : MonoBehaviour
{
   
    private GameObject Perkshop;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Perkshop = GameObject.Find("PerkShopUI");
        if (Perkshop != null)
        {
            Perkshop.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
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
        if (Perkshop != null)
        {
            if (other.CompareTag("Player"))
            {
                Perkshop.SetActive(false);
            }
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
