using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NoteUIManager : MonoBehaviour
{
    public static NoteUIManager instance;

    public GameObject notePanel;
    public TextMeshProUGUI noteText;
    public TextMeshProUGUI hintText;

    private void Awake()
    {
        instance = this;
        notePanel.SetActive(false);
        hintText.gameObject.SetActive(false);
    }

    public void OpenNote()
    {
        notePanel.SetActive(true);
        gameManager.instance.PauseGame();
        gameManager.instance.menuActive = notePanel;
    }

    public void CloseNote()
    {
        notePanel.SetActive(false);
        gameManager.instance.ResumeGame();
    }

    public void ShowHint(bool show)
    {
        hintText.gameObject.SetActive(show);
    }
}