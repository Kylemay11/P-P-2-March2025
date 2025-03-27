using UnityEngine;


public class perkShop : MonoBehaviour
{
    public static perkShop instance;

    [Header("Prices")]
    [Range(1, 100)][SerializeField] public int SpeedPrice;
    [Range(1, 100)][SerializeField] public int BonushealthPrice;
    [Range(1, 500)][SerializeField] public int HitDamagePrice;
    [Range(1, 500)][SerializeField] public int IAmmoPrice;
    [Range(1, 500)][SerializeField] public int RandPerkPrice;

    [Header("Variables")]
    [Range(50, 1000)][SerializeField] public int bonusHP;
    [Range(1, 50)][SerializeField] float bonusSpeed;
    [Range(1, 100)][SerializeField] public int bonusDamage;



    //Purchases
    public void makeIncresDamagePurchase()
    {
        if (CurrencySystem.instance.SpendMoney(HitDamagePrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<raycastWeapon>()?.DamageIncrease(bonusDamage);
            Debug.Log("You have increased damage");
        }
    }

    public void makeIncresSpeedPurchase()
    {
        if (CurrencySystem.instance.SpendMoney(SpeedPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.SpeedIncrease(bonusSpeed);
            Debug.Log("You have increased speed");

        }
    }

    public void makeBonushealthPurchase()
    {
        if (CurrencySystem.instance.SpendMoney(SpeedPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.BonusHealth(bonusHP);
            Debug.Log("You have increased max HP");
        }
    }
    //public void makeInfiniteAmmoPurchase()
    //{
    //    if (CurrencySystem.instance.SpendMoney(IAmmoPrice))
    //    {
    //        GameObject player = GameObject.FindGameObjectWithTag("Player");
    //        player.GetComponent<raycastWeapon>()?.InfiniteAmmoPerk();
    //    }
    //}


    //perks for random perk
    public void IncresDamagePerk()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<raycastWeapon>()?.DamageIncrease(bonusDamage);
    }
    public void IncresSpeedPerk()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<playerController>()?.SpeedIncrease(bonusSpeed);
    }
    public void BonushealthPerk()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<playerController>()?.BonusHealth(bonusHP);
    }


    public void randomPerk()
    {
        if (CurrencySystem.instance.SpendMoney(RandPerkPrice))
        {
            int randomPerk = Random.Range(0, 3);
            switch (randomPerk)
            {
                case 0:
                    IncresDamagePerk();
                    Debug.Log("You have increased damage");
                    break;
                case 1:
                    IncresSpeedPerk();
                    Debug.Log("You have increased speed");
                    break;
                case 2:
                    BonushealthPerk();
                    Debug.Log("You have increased max HP");
                    break;
                //case 3:
                // makeInfiniteAmmoPurchase();
                // break;
                default:
                    Debug.LogError("Invalid perk selection.");
                    break;
            }
        }
    }

}
