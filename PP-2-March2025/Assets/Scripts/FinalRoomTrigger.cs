using UnityEngine;

public class FinalRoomTrigger : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject exitPromptUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (exitPromptUI != null)
                exitPromptUI.SetActive(true);

            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OnLeaveMap()
    {
        Time.timeScale = 1f;
        gameManager.instance.YouWin(); // Or load another scene
        if (exitPromptUI != null)
            exitPromptUI.SetActive(false);
    }

    public void OnKeepFighting()
    {
        if (exitPromptUI != null)
            exitPromptUI.SetActive(false);

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}