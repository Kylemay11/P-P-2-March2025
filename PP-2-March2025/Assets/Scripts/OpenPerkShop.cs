using UnityEngine;


public class OpenPerkShop : MonoBehaviour
{
    private GameObject Perkshop;
    private bool isActive = false;
    public bool isPlayerNear = false;

    void Start()
    {
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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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

    void TogglePerkShop()
    {
        isActive = !isActive;
        if (Perkshop != null)
            Perkshop.SetActive(isActive);
    }

    void untogglePerkShop()
    {
        
        if (Perkshop != null)
            Perkshop.SetActive(false);
    }
}