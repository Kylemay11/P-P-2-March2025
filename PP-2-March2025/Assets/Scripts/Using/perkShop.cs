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
    [Range(1, 100)][SerializeField] public int bonusStaminaPrice;
    [Range(1, 500)][SerializeField] public int Smallhealthpack;
    [Range(1, 500)][SerializeField] public int Healthpack;

    [Header("Variables")]
    [Range(50, 1000)][SerializeField] public int bonusHP;
    [Range(1, 50)][SerializeField] public float bonusSpeed;
    [Range(1, 100)][SerializeField] public int bonusStamina;

    [Range(1, 100)][SerializeField] public int bonusDamage;
    [Range(1, 1000)][SerializeField] public int bonusWepDist;
    [Range(0.1f, 5)][SerializeField] public float bonusWepRate;

    // might not be able to do
    [Range(1, 100)][SerializeField] public int bonusMeleeDamage;
    [Range(1, 100)][SerializeField] public int bonusMeleeRate;
    [Range(1, 100)][SerializeField] public int bonusMeleeDist;



    [Range(1, 100)][SerializeField] public int smallhealthpackPrice;
    [Range(1, 100)][SerializeField] public int healthpackPrice;



    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    //Purchases
    public void makeBonushealthPurchase()
    {
        if (CurrencySystem.instance.SpendMoney(SpeedPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.BonusHealth(bonusHP);
            player.GetComponent<playerController>()?.Heal(bonusHP);
            Debug.Log("You have increased max HP");
            gameManager.instance.UnlockHealthPerk();
        }
    }
    public void makeIncresStaminaPurchase()
    {
        if (CurrencySystem.instance.SpendMoney(bonusStaminaPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.StaminaIncrease(bonusStamina);
            Debug.Log("You have increased stamina");
            gameManager.instance.UnlockStaminaPerk();
        }
    }
    public void makeIncresSpeedPurchase()
    {
        if (CurrencySystem.instance.SpendMoney(SpeedPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.SpeedIncrease(bonusSpeed);
            Debug.Log("You have increased speed");
            gameManager.instance.UnlockSpeedPerk();
        }
    }
    public void makeIncresDamagePurchase()
    {
        if (CurrencySystem.instance.SpendMoney(HitDamagePrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.DamageIncrease(bonusDamage);
            Debug.Log("You have increased damage");
            gameManager.instance.UnlockGunDamagePerk();
        }
    }


    public void makeIncresWeaponRatePurchase()
    {
        if (CurrencySystem.instance.SpendMoney(SpeedPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.IncreaseWeaponFireRate(bonusWepRate);
            Debug.Log("You have increased speed");
            gameManager.instance.UnlockGunDamagePerk();
        }
    }

    public void makeIncresWeaponDistPurchase()
    {
        if (CurrencySystem.instance.SpendMoney(SpeedPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.IncreaseWeaponFireDistance(bonusWepDist);
            Debug.Log("You have increased speed");
           gameManager.instance.UnlockGunDamagePerk();
        }
    }

    //public void makeIncresMeleeDamagePurchase()
    //{
    //    if (CurrencySystem.instance.SpendMoney(SpeedPrice))
    //    {
    //        GameObject player = GameObject.FindGameObjectWithTag("Player");
    //        player.GetComponent<playerController>()?.SpeedIncrease(bonusSpeed);
    //        Debug.Log("You have increased speed");
            
    //    }
    //}

    //public void makeIncresMeleeRatePurchase()
    //{
    //    if (CurrencySystem.instance.SpendMoney(SpeedPrice))
    //    {
    //        GameObject player = GameObject.FindGameObjectWithTag("Player");
    //        player.GetComponent<playerController>()?.SpeedIncrease(bonusSpeed);
    //        Debug.Log("You have increased speed");
            
    //    }
    //}

    //public void makeIncresMeleeDistPurchase()
    //{
    //    if (CurrencySystem.instance.SpendMoney(SpeedPrice))
    //    {
    //        GameObject player = GameObject.FindGameObjectWithTag("Player");
    //        player.GetComponent<playerController>()?.SpeedIncrease(bonusSpeed);
    //        Debug.Log("You have increased speed");
           
    //    }
    //}

    public void makesmallhpPurchase()
    {
        if (CurrencySystem.instance.SpendMoney(smallhealthpackPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.Heal(Smallhealthpack);
            player.GetComponent<playerController>()?.updatePlayerUI();
        }
    }

    public void makehpPurchase()
    {
        if (CurrencySystem.instance.SpendMoney(healthpackPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.Heal(Healthpack);
            player.GetComponent<playerController>()?.updatePlayerUI();
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
        player.GetComponent<playerController>()?.DamageIncrease(bonusDamage);
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
    public void BonusStaminaPerk()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<playerController>()?.StaminaIncrease(bonusStamina);
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
                case 3:
                    BonusStaminaPerk();
                    Debug.Log("You have increased stamina");
                    break;
                default:
                    Debug.LogError("Invalid perk selection.");
                    break;
            }
        }
    }

}
