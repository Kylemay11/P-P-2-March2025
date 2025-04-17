using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool isActive = false;
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

    public void playGame(int lvlID)
    {
        string levelname = "Level_" + lvlID;
        SceneManager.LoadScene(levelname);
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
        isActive = !isActive;
        if (credReel != null)
            credReel.SetActive(isActive);
    }

    public void openSettings()
    {
        isActive = !isActive;
        if (settingsUI != null)
            settingsUI.SetActive(isActive);
    }
    public void openLevelSelector()
    {
        isActive = !isActive;
        if (lvlSelect != null)
            lvlSelect.SetActive(isActive);
    }

}
