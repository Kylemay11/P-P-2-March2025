using UnityEngine;

public class NoteTrigger : MonoBehaviour
{
    private bool isPlayerNear = false;

    private void Update()
    {
        if (isPlayerNear && Input.GetButtonDown("Interact"))
        {
            if (NoteUIManager.instance != null)
            {
                NoteUIManager.instance.OpenNote();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            NoteUIManager.instance.ShowHint(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            NoteUIManager.instance.ShowHint(false);
        }
    }
}