
using System.Collections.Generic;
using UnityEngine;



public class Shop : MonoBehaviour
{
    [Range(1, 50)][SerializeField] int smallhealthpackPrice;
    [Range(1, 100)][SerializeField] int healthpackPrice;
    [Range(1, 500)][SerializeField] int largehealthpackPrice;

    [Range(1, 100)][SerializeField] int smgAmmoPrice;
    [Range(1, 100)][SerializeField] int ShotgunShellPrice;
    [Range(1, 100)][SerializeField] int PistolAmmoPrice;
    [Range(1, 1000)][SerializeField] int SpecialAmmoPrice;

    [Range(1, 1000)][SerializeField] int SMGPrice;
    [Range(1, 1000)][SerializeField] int ShotgunPrice;
    [Range(1, 1000)][SerializeField] int BFGPrice;
    [Range(1, 1000)][SerializeField] int RYNOPrice;
    [Range(1, 1000)][SerializeField] int WoodBatPrice;

    private int Smallhealthpack;
    private int Healthpack;
    private int Largehealthpack;

    private int SmgAmmo;
    private int ShotgunShells;
    private int SpecialAmmo;

    private int SMG;
    private int Shotgun;
    private int BFG;
    private int RYNO;
    private int WOODBAT;

    void Start()
    {
     
    }

    void makePurchase()
    {
       if(CurrencySystem.instance.currentMoney >= smallhealthpackPrice)
        {
            Smallhealthpack++;
            CurrencySystem.instance.currentMoney -= smallhealthpackPrice;
            print("Hurry take your small health potion, guess you now got " + Smallhealthpack.ToString());
        }
       else
        {
            print("Come back when ya got the money to spend");
        }

       if (CurrencySystem.instance.currentMoney >= healthpackPrice)
        {
            Healthpack++;
            CurrencySystem.instance.currentMoney -= healthpackPrice;
            print("Hurry take your Health potion, guess you now got " + Healthpack.ToString());
        }
        else
        {
            print("Come back when ya got the money to spend");
        }

        if (CurrencySystem.instance.currentMoney >= largehealthpackPrice)
        {
            Largehealthpack++;
            CurrencySystem.instance.currentMoney -= largehealthpackPrice;
            print("Hurry take your Large health potion, guess you now got " + Largehealthpack.ToString());
        }
        else
        {
            print("Come back when ya got the money to spend");
        }


        if (CurrencySystem.instance.currentMoney >= smgAmmoPrice)
        {
            SmgAmmo++;
            CurrencySystem.instance.currentMoney -= smgAmmoPrice;
            print("Hurry take your SMG ammo, guess you now got " + SmgAmmo.ToString());
        }
        else
        {
            print("Come back when ya got the money to spend");
        }
        if (CurrencySystem.instance.currentMoney >= ShotgunShellPrice)
        {
            ShotgunShells++;
            CurrencySystem.instance.currentMoney -= ShotgunShellPrice;
            print("Hurry take your Shotgun shells, guess you now got " + ShotgunShells.ToString());
        }
        else
        {
            print("Come back when ya got the money to spend");
        }
        if (CurrencySystem.instance.currentMoney >= SpecialAmmoPrice)
        {
           SpecialAmmo++;
            CurrencySystem.instance.currentMoney -= SpecialAmmoPrice;
            print("Hurry take your special ammo, guess you now got " + SpecialAmmo.ToString());
        }
        else
        {
            print("Come back when ya got the money to spend");
        }


        if (CurrencySystem.instance.currentMoney >= SMGPrice)
        {
            SMG++;
            CurrencySystem.instance.currentMoney -= SMGPrice;
            print("Wow now you got a " + SMG.ToString());
        }
        else
        {
            print("Come back when ya got the money to spend");
        }
        if (CurrencySystem.instance.currentMoney >= ShotgunPrice)
        {
            Shotgun++;
            CurrencySystem.instance.currentMoney -= ShotgunPrice;
            print("Wow now you got a " + Shotgun.ToString());
        }
        else
        {
            print("Come back when ya got the money to spend");
        }
        if (CurrencySystem.instance.currentMoney >= BFGPrice)
        {
            BFG++;
            CurrencySystem.instance.currentMoney -= BFGPrice;
            print("Wow now you got a " + BFG.ToString());
        }
        else
        {
            print("Come back when ya got the money to spend");
        }
        if (CurrencySystem.instance.currentMoney >= RYNOPrice)
        {
            RYNO++;
            CurrencySystem.instance.currentMoney -= RYNOPrice;
            print("Wow now you got a " +    RYNO.ToString());
        }
        else
        {
            print("Come back when ya got the money to spend");
        }

        if (CurrencySystem.instance.currentMoney >= WoodBatPrice)
        {
            WOODBAT++;
            CurrencySystem.instance.currentMoney -= WoodBatPrice;
            print("Wow now you got a " + WOODBAT.ToString());
        }
        else
        {
            print("Come back when ya got the money to spend");
        }

    }

}
