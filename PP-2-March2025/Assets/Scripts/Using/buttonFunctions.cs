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
        gameManager.instance.PauseGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.ResumeGame();
    }


    public void quit()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}