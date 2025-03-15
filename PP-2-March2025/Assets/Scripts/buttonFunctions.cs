using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{

    public void resume()
    {
        gameManager.instance.ResumeGame();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.PauseGame();
    }


    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }



    public void smallhealth()
    {
        Shop.instance.makeshpPurchase();
    }

    public void Health()
    {
        Shop.instance.makehpPurchase();
    }

    public void largeHealth()
    {
        Shop.instance.makelhpPurchase();
    }

    public void smgammo()
    {
        Shop.instance.makesmgAmmoPurchase();
    }
    public void Pgammo()
    {
        Shop.instance.makePAmmoPurchase();
    }
    public void SShells()
    {
        Shop.instance.makeSSPurchase();
    }
    public void SPammo()
    {
        Shop.instance.makeSAmmoPurchase();
    }
    public void SMG()
    {
        Shop.instance.makesmgPurchase();
    }
    public void Shotgun()
    {
        Shop.instance.makeShotgunPurchase();
    }
    public void bfg()
    {
        Shop.instance.makeBFGPurchase();
    }
    public void RYNO()
    {
        Shop.instance.makeRYNOPurchase();
    }
    public void WB()
    {
        Shop.instance.makeWBPurchase();
    }



    public void BonusHealth()
    {
        perkShop.instance.makeBonushealthPurchase();
    }
    public void Speed()
    {
        perkShop.instance.makeSpeedPurchase();
    }
    public void Damage()
    {
        perkShop.instance.makeHDamagePurchase();
    }
}