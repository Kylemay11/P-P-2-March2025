using UnityEngine;
using UnityEngine.UI;

public class RepairConsole : MonoBehaviour
{
    public float repairTime;
    public float decayOverTime;
    private float currentProgress = 0f;
    private bool isRepaired;
    private bool playerInRange;

    [Header("UI")]
    public Image repairBar;

    void Update()
    {
        if (isRepaired) return;

        if (playerInRange && Input.GetKey(KeyCode.E))
        {
            currentProgress += Time.deltaTime;
        }
        else
        {
            currentProgress -= Time.deltaTime * decayOverTime;
        }

        currentProgress = Mathf.Clamp(currentProgress, 0, repairTime);

        if (repairBar != null)
            repairBar.fillAmount = currentProgress / repairTime;

        if (currentProgress >= repairTime && !isRepaired)
        {
            CompleteRepair();
        }
    }

    private void CompleteRepair()
    {
        isRepaired = true;
        Debug.Log($"{gameObject.name} repaired!");
        if (repairBar != null)
            repairBar.color = Color.green;
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