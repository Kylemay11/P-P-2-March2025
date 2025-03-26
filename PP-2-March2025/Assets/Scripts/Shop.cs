
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using UnityEngine;



public class Shop : MonoBehaviour
{
    public static Shop instance;

    [Range(1, 50)][SerializeField] int smallhealthpackPrice;
    [Range(1, 100)][SerializeField] int healthpackPrice;
    [Range(1, 500)][SerializeField] int largehealthpackPrice;

    [Range(1, 500)][SerializeField] int AmmoPrice;

    [Range(1, 500)][SerializeField] int AmmoAmount;

    private int Smallhealthpack;
    private int Healthpack;
    private int Largehealthpack;

    


    // Vending Machine
   public void makesmallhpPurchase()
    {
        if (CurrencySystem.instance.SpendMoney(smallhealthpackPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.Heal(Smallhealthpack);
           
        }
    }
    public void makehpPurchase()
    {
        if (CurrencySystem.instance.SpendMoney(healthpackPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.Heal(Healthpack);
        }
    }
    public void makelhpPurchase()
    {
        if (CurrencySystem.instance.SpendMoney(largehealthpackPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.Heal(Largehealthpack);
        }
    }

public void makeAmmoPurchase()
    {
        if (CurrencySystem.instance.SpendMoney(AmmoPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<raycastWeapon>()?.AmmoIncrease(AmmoAmount);
        }
    }
   
    

    // Weapon shop


}
