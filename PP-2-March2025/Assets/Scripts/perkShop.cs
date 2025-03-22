using UnityEngine;

public class perkShop : MonoBehaviour
{
    public static perkShop instance;

    [Range(1, 100)][SerializeField] int SpeedPrice;
    [Range(1, 100)][SerializeField] int BonushealthPrice;
    [Range(1, 500)][SerializeField] int HitDamagePrice;
    [Range(50, 1000)][SerializeField] public int BonusHP;
    [Range(1, 50)][SerializeField] public int bonusSpeed;
    [Range(1, 100)][SerializeField] public int bonusDamage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        public void makeIncresDamagePurchase()
        {
        if (CurrencySystem.instance != null && raycastWeapon.instance != null)
        {
            if (CurrencySystem.instance.currentMoney >= HitDamagePrice)
            {
                raycastWeapon.instance.DamageIncrease();
                CurrencySystem.instance.currentMoney -= HitDamagePrice;
                print("Now your Damage should be stronger");
                CurrencySystem.instance.currentMoney--;
            }
            else
            {
                print("Come back when ya got the money to spend");
            }
        }
        }
        public void makeIncresSpeedPurchase()
        {
        if (CurrencySystem.instance != null && playerController.instance != null)
        {
            if (CurrencySystem.instance.currentMoney >= SpeedPrice)
            {
                playerController.instance.SpeedIncrease();
                CurrencySystem.instance.currentMoney -= SpeedPrice;
                print("Now your Walk should be faster");
                CurrencySystem.instance.currentMoney--;
            }
            else
            {
                print("Come back when ya got the money to spend");
            }
        }
        }
        public void makeBonushealthPurchase()
        {
        if (CurrencySystem.instance != null && playerController.instance != null)
        {
            if (CurrencySystem.instance.currentMoney >= BonushealthPrice)
            {
                playerController.instance.BonusHealth();
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
}
