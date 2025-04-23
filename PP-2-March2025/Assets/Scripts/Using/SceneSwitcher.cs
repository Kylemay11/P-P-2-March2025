using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string sceneToLoad = "John's Scene";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            loadScene();
        }
    }

    private void loadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
