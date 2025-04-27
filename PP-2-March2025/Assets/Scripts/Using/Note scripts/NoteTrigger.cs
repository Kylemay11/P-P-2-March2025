using UnityEngine;

public class NoteTrigger : MonoBehaviour
{
    [TextArea(3, 10)]
    public string noteText;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetButtonDown("Interact"))
        {
            OpenNote();
        }
    }

    void OpenNote()
    {
        NoteUIManager.instance.OpenNote(noteText);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}