using UnityEngine;


public class perkShop : MonoBehaviour
{
    public static perkShop instance;


    [Range(1, 100)][SerializeField] public int SpeedPrice;
    [Range(1, 100)][SerializeField] public int BonushealthPrice;
    [Range(1, 500)][SerializeField] public int HitDamagePrice;
    [Range(50, 1000)][SerializeField] public int bonusHP;
    [Range(1, 50)][SerializeField] public int  bonusSpeed;
    [Range(1, 100)][SerializeField] public int bonusDamage;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null) player = GameObject.FindWithTag("Player");
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

            Debug.Log("Now your Walk should be faster");

        }
    }
        public void makeIncresSpeedPurchase()
        {


        if (CurrencySystem.instance.SpendMoney(SpeedPrice))
           {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.SpeedIncrease(bonusSpeed);

            Debug.Log("Now your Walk should be faster");
           }
     

        }
        public void makeBonushealthPurchase()
        {
       
            if (CurrencySystem.instance.SpendMoney(SpeedPrice))
            {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.BonusHealth(bonusHP);

            Debug.Log("Now your Walk should be faster");
        }        
        }

}
