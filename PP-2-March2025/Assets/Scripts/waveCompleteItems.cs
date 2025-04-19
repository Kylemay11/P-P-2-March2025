using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class waveCompleteItems : MonoBehaviour
{
    public static waveCompleteItems instance;
    public GameObject[] panels;
    [SerializeField] public float interval = 60f;
    private float timer;
    [Range(50, 1000)][SerializeField] public int bonusHP;
    [Range(1, 50)][SerializeField] public float bonusSpeed;
    [Range(1, 100)][SerializeField] public int bonusDamage;
    [Range(1, 100)][SerializeField] public int bonusStamina;
    [Range(50, 1000)][SerializeField] public int bonusWepDist;
    [Range(1, 50)][SerializeField] public int bonusWepRate;
    [Range(1, 100)][SerializeField] public int bonusMeleeDamage;
    [Range(1, 100)][SerializeField] public int bonusMeleeRate;
    [Range(1, 100)][SerializeField] public int bonusMeleeDist;


    void Start()
    {
        timer = interval;
        HideAllPanels();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            ShowRandomPanel();
            timer = interval;
        }
    }

    void ShowRandomPanel()
    {
        gameManager.instance.PauseGame();
        HideAllPanels();
        

        int index = Random.Range(0, panels.Length);
        panels[index].SetActive(true);
    }

    void HideAllPanels()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }


    public void makeIncresDamagePurchase()
    {
        HideAllPanels();
        gameManager.instance.ResumeGame();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<playerController>()?.DamageIncrease(bonusDamage);
        Debug.Log("You have increased damage");



    }

    public void makeIncresStaminaPurchase()
    {
        HideAllPanels();
        gameManager.instance.ResumeGame();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<playerController>()?.StaminaIncrease(bonusStamina);
        Debug.Log("You have increased stamina");

    }
    public void makeIncresSpeedPurchase()
    {
        HideAllPanels();
        gameManager.instance.ResumeGame();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<playerController>()?.SpeedIncrease(bonusSpeed);
        Debug.Log("You have increased speed");


    }

    public void makeIncresWeaponRatePurchase()
    {
        HideAllPanels();
        gameManager.instance.ResumeGame();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<playerController>()?.IncreaseWeaponFireRate(bonusWepDist);
        Debug.Log("You have increased Gun Shot Rate of Fire");

    }

    public void makeIncresWeaponDistPurchase()
    {
        HideAllPanels();
        gameManager.instance.ResumeGame();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<playerController>()?.IncreaseWeaponFireDistance(bonusWepRate);
        Debug.Log("You have increased Gun Shot Distance");

    }

    public void makeIncresMeleeDamagePurchase()
    {
        HideAllPanels();
        gameManager.instance.ResumeGame();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<playerController>()?.SpeedIncrease(bonusMeleeDamage);
        Debug.Log("You have increased Melee Damage");

    }

    public void makeIncresMeleeRatePurchase()
    {
        HideAllPanels();
        gameManager.instance.ResumeGame();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<playerController>()?.SpeedIncrease(bonusMeleeRate);
        Debug.Log("You Attack Faster");

    }

    public void makeIncresMeleeDistPurchase()
    {
        HideAllPanels();
        gameManager.instance.ResumeGame();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<playerController>()?.SpeedIncrease(bonusMeleeDist);
        Debug.Log("You have longer reach to your attack");

    }
    public void makeBonushealthPurchase()
    {
        HideAllPanels();
        gameManager.instance.ResumeGame();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<playerController>()?.BonusHealth(bonusHP);
        Debug.Log("You have increased max HP");

    }
}