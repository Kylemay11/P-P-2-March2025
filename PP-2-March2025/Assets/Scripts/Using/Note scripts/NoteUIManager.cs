using UnityEngine;
using TMPro;

public class NoteUIManager : MonoBehaviour
{
    public static NoteUIManager instance;

    [Header("References")]
    public GameObject notePanel;
    public TMP_Text noteTextField;
    public TMP_Text bottomHintText;
    public TMP_Text interactPrompt;

    void Awake()
    {
        instance = this;
        notePanel.SetActive(false);
    }

    public void OpenNote(string text)
    {
        noteTextField.text = text;
        notePanel.SetActive(true);
        gameManager.instance.menuActive = notePanel;
        gameManager.instance.PauseGame();
    }

    public void CloseNote()
    {
        notePanel.SetActive(false);
        gameManager.instance.menuActive = null;
        gameManager.instance.ResumeGame();
    }
}