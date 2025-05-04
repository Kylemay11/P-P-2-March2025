using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public static MainMenu instance;
    public GameObject credReel;
    public GameObject settingsUI;
    public GameObject lvlSelect;

    void Start()
    {
  
        credReel = GameObject.Find("Reel");
        if (credReel != null)
        {
            credReel.SetActive(false);
        }

        lvlSelect = GameObject.Find("Level Selector");
        if (lvlSelect != null)
        {
            lvlSelect.SetActive(false);
        }

        settingsUI = GameObject.Find("Setting");
        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }

    }
    public void Update()
    {
       
    }

    public void playGame(int lvlID)
    {
        string levelname = "Level_" + lvlID;
        SceneManager.LoadScene(levelname);
        gameManager.instance.ResumeGame();
        OpenPerkShop.Instance.isInShop = false;
        OpenWshop.Instance.isInShop = false;
        OpenShop.instance.isInShop = false;
    }

    public void gameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void creditsReel()
    {
        gameManager.instance.menuCredits.SetActive(true);
    }
    public void closecreditsReel()
    {
        gameManager.instance.menuCredits.SetActive(false);
    }
    public void openSettings()
    {
        gameManager.instance.menusettings.SetActive(true);
    }
    public void closeSettings()
    {
        gameManager.instance.menusettings.SetActive(false);
    }
    public void openLevelSelector()
    {
        gameManager.instance.menuLvlSelect.SetActive(true);
    }

    public void closeLevelSelector()
    {
        gameManager.instance.menuLvlSelect.SetActive(false);
    }
    public void openMainMenu()
    {
        string levelname = "Main Menu";
        SceneManager.LoadScene(levelname);
    }
}
