using UnityEngine;


public class perkShop : MonoBehaviour
{
    public static perkShop instance;


    [Range(1, 100)][SerializeField] public int SpeedPrice;
    [Range(1, 100)][SerializeField] public int BonushealthPrice;
    [Range(1, 500)][SerializeField] public int HitDamagePrice;
    [Range(50, 1000)][SerializeField] public int bonusHP;
    [Range(1, 50)][SerializeField] float bonusSpeed;
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


        if (CurrencySystem.instance.SpendMoney(HitDamagePrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<raycastWeapon>()?.DamageIncrease(bonusDamage);

        }
    }
    public void makeIncresSpeedPurchase()
    {


        if (CurrencySystem.instance.SpendMoney(SpeedPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            // player.GetComponent<playerController>()?.SpeedIncrease(bonusSpeed);
        }


    }
    public void makeBonushealthPurchase()
    {

        if (CurrencySystem.instance.SpendMoney(SpeedPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            // player.GetComponent<playerController>()?.BonusHealth(bonusHP);
        }
    }

}
