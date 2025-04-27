using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class FuseBox : MonoBehaviour
{
    [Header("Repair Settings")]
    [SerializeField] private float repairTime;
    private float currentProgress;
    private bool isRepaired = false;
    private bool isPlayerNear = false;

    [Header("Material Swap")]
    [SerializeField] private List<MeshRenderer> objectsToSwap;
    [SerializeField] private Material greenMaterial;

    [Header("References")]
    [SerializeField] private GameObject repairPromptUI;
    [SerializeField] private TextMeshProUGUI repairPromptText;
    [SerializeField] private Image repairBar;
    [SerializeField] private DoorInteract doorToUnlock; // the door you unlock after repairing

    private void Start()
    {
        if (repairPromptUI != null)
            repairPromptUI.SetActive(false);

        if (repairBar != null)
            repairBar.fillAmount = 0f;
    }

    private void Update()
    {
        if (isPlayerNear && !isRepaired)
        {
            if (Input.GetButton("Interact"))
            {
                currentProgress += Time.deltaTime;
                if (repairBar != null)
                    repairBar.fillAmount = currentProgress / repairTime;

                if (currentProgress >= repairTime)
                {
                    CompleteRepair();
                }
            }
        }
    }

    private void CompleteRepair()
    {
        isRepaired = true;
        if (repairPromptUI != null)
            repairPromptUI.SetActive(false);

        if (repairBar != null)
            repairBar.fillAmount = 1f;

        foreach (MeshRenderer obj in objectsToSwap)
        {
            if (obj != null)
                obj.material = greenMaterial;
        }

        Debug.Log("Fuse box repaired!");
        if (doorToUnlock != null)
            doorToUnlock.CompleteEvent();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isRepaired /*&& gameManager.instance.waveActive*/)
        {
            isPlayerNear = true;
            if (repairPromptUI != null)
            {
                repairPromptUI.SetActive(true);
                if (repairPromptText != null)
                    repairPromptText.text = "Hold [E] to Repair Fuse Box";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (repairPromptUI != null)
                repairPromptUI.SetActive(false);
        }
    }
}