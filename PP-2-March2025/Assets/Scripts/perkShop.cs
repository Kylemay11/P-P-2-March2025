using UnityEngine;

public class perkShop : MonoBehaviour
{
    public static perkShop instance;

    [Range(1, 50)][SerializeField] int SpeedPrice;
    [Range(1, 100)][SerializeField] int BonushealthPrice;
    [Range(1, 500)][SerializeField] int HitDamagePrice;


    [Range(1, 10)][SerializeField] int Speed;
    [Range(1, 1000)][SerializeField] int Bonushealth;
    [Range(1, 50)][SerializeField] int HitDamage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void makeHDamagePurchase()
    {
        
        if (CurrencySystem.instance.currentMoney >= HitDamagePrice)
        {
            raycastWeapon.instance.damage+= HitDamage;
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
            playerController.instance.walkSpeed+= Speed;
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
            playerController.instance.maxHP+= Bonushealth;
            CurrencySystem.instance.currentMoney -= BonushealthPrice;
            print("Now your Health should be bigger");
            CurrencySystem.instance.currentMoney--;
        }
        else
        {
            print("Come back when ya got the money to spend");
        }
    }
}
