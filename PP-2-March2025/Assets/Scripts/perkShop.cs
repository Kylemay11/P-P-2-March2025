using UnityEngine;

public class perkShop : MonoBehaviour
{
    public static perkShop instance;

    [Range(1, 50)][SerializeField] public int SpeedPrice;
    [Range(1, 100)][SerializeField] int BonushealthPrice;
    [Range(1, 500)][SerializeField] int HitDamagePrice;
    //temp
    [SerializeField] int ammoUPPrice;
    [SerializeField] int ammoUP;
    [SerializeField] bool isInteract;
    public GameObject interactionUI;


    [Range(1, 10)][SerializeField] int Speed;
    [Range(1, 1000)][SerializeField] int Bonushealth;
    [Range(1, 50)][SerializeField] int HitDamage;
    GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null) player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void makeHDamagePurchase()
    {

        if (CurrencySystem.instance.currentMoney >= HitDamagePrice)
        {
            raycastWeapon.instance.damage += HitDamage;
            CurrencySystem.instance.currentMoney -= HitDamagePrice;
            print("Now your Damage should be stronger");
            CurrencySystem.instance.currentMoney--;
        }
        else
        {
            print("Come back when ya got the money to spend");
        }
    }
    public void makeSpeedPurchase()
    {
        if (CurrencySystem.instance.currentMoney >= SpeedPrice)
        {
            playerController.instance.walkSpeed += Speed;
            CurrencySystem.instance.currentMoney -= SpeedPrice;
            print("Now your Walk should be faster");
            CurrencySystem.instance.currentMoney--;
        }
        else
        {
            print("Come back when ya got the money to spend");
        }
    }
    public void makeBonushealthPurchase()
    {
        if (CurrencySystem.instance.currentMoney >= BonushealthPrice)
        {
            playerController.instance.maxHP += Bonushealth;
            CurrencySystem.instance.currentMoney -= BonushealthPrice;
            print("Now your Health should be bigger");
            CurrencySystem.instance.currentMoney--;
        }
        else
        {
            print("Come back when ya got the money to spend");
        }
    }
    
    public void tempSpeedBoost()
    {
        if (CurrencySystem.instance.SpendMoney(SpeedPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.speedBoost(Speed);
        }
    }

    public void tempHeal()
    {
        if (CurrencySystem.instance.SpendMoney(BonushealthPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.Heal(Bonushealth);
        }
    }

    public void tempAmmoUp()
    {
        if(CurrencySystem.instance.SpendMoney(ammoUPPrice))
        {
            raycastWeapon gun = raycastWeapon.FindAnyObjectByType<raycastWeapon>();
            gun.GetComponent<raycastWeapon>()?.ammoUP(ammoUP);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInteract = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (interactionUI != null)
                interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInteract = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (interactionUI != null)
                interactionUI.SetActive(false);
        }
    }

}
