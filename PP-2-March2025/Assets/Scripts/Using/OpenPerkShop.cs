using UnityEngine;


public class OpenPerkShop : MonoBehaviour
{
    private GameObject Perkshop;
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
        gameManager.instance.menuPerkShop.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //gameManager.instance.PauseGame();
    }

    public void untogglePerkShop()
    {
        gameManager.instance.menuPerkShop.SetActive(false);
       // gameManager.instance.ResumeGame();
    }
}