
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using UnityEngine;



public class Shop : MonoBehaviour
{
    public static Shop instance;

    [Header("Prices")]
    [Range(1, 50)][SerializeField] int smallhealthpackPrice;
    [Range(1, 250)][SerializeField] int healthpackPrice;
    [Range(1, 500)][SerializeField] int largehealthpackPrice;

    [Range(10, 500)][SerializeField] int weapon1Price;
    [Range(10, 500)][SerializeField] int weapon2Price;
    [Range(10, 500)][SerializeField] int weapon3Price;
    [Range(10, 500)][SerializeField] int weapon4Price;

    [Range(1, 500)][SerializeField] int AmmoPrice;

    [Header("Amounts")]
    [Range(1, 100)][SerializeField] int AmmoAmount;
    [Range(1, 100)][SerializeField] int Smallhealthpack;
    [Range(1, 100)][SerializeField] int Healthpack;
    [Range(1, 100)][SerializeField] int Largehealthpack;


    [SerializeField] private List<weaponStats> availableWeapons = new List<weaponStats>(); // Weapons available in the shop
    [SerializeField] private int weaponReplaceIndex = 0;

    // Vending Machine
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
    public void makelhpPurchase()
    {
        if (CurrencySystem.instance.SpendMoney(largehealthpackPrice))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<playerController>()?.Heal(Largehealthpack);
            player.GetComponent<playerController>()?.updatePlayerUI();
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

    public void PurchaseWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= availableWeapons.Count)
        {
            return;
        }

        weaponStats newWeapon = availableWeapons[weaponIndex];
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<playerController>()?.ReplaceWeapon(newWeapon, weaponReplaceIndex);
    }

    // purchase the weapons from their corrsponding postion in the list
    public void PurchaseWeapon1()
    {
        PurchaseWeapon(0);
    }

    public void PurchaseWeapon2()
    {
        PurchaseWeapon(1);
    }

    public void PurchaseWeapon3()
    {
        PurchaseWeapon(2);
    }

    public void PurchaseWeapon4()
    {
        PurchaseWeapon(3);
    }


}
