using UnityEngine;

public class Switch : MonoBehaviour
{
    public Material greenMat;
    public Renderer switchRenderer;
    public SwitchManager manager;
    private bool isFlipped = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isFlipped && Input.GetButtonDown("Interact"))
        {
            FlipSwitch();
        }
    }

    private void FlipSwitch()
    {
        isFlipped = true;
        if (switchRenderer != null && greenMat != null)
            switchRenderer.material = greenMat;

        if (manager != null)
            manager.SwitchFlipped();
    }
}