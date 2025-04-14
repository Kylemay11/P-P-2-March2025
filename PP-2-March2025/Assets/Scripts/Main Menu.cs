using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool isActive = false;
    public GameObject credReel;

    void Start()
    {

        credReel = GameObject.Find("Reel");
        if (credReel != null)
        {
            credReel.SetActive(false);
        }

    }

    public void playGame()
    {
        SceneManager.LoadScene("Kyle's_Scene");
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

    }



}
