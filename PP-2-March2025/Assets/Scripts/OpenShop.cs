using UnityEngine;

public class OpenShop : MonoBehaviour
{
    private GameObject shop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shop = GameObject.Find("ShopUI");
        shop.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && gameManager.instance.waveActive == false)
        {
            shop.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            shop.SetActive(false);
        }
    }
}
