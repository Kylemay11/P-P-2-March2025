using TMPro;
using UnityEngine;

public class TutorialChecklistUI : MonoBehaviour
{
    public TextMeshProUGUI checklistText;

    private bool[] objectivesCompleted;

    private string[] objectiveDescriptions = new string[]
    {
        "Repair barricade",
        "Unlock the firing range door",
        "Shoot a target dummy",
        "Charge the terminal",
        "Open the door and start the game"
    };

    void Start()
    {
        objectivesCompleted = new bool[objectiveDescriptions.Length];
        UpdateChecklistText();
    }

    public void CompleteObjective(int index)
    {
        if (index >= 0 && index < objectivesCompleted.Length)
        {
            objectivesCompleted[index] = true;
            UpdateChecklistText();
        }
    }

    void UpdateChecklistText()
    {
        string updatedText = "Tutorial checklist\n\n";

        for (int i = 0; i < objectiveDescriptions.Length; i++)
        {
            string checkbox = objectivesCompleted[i] ? "[X]" : "[ ]";
            updatedText += $"{objectiveDescriptions[i]} {checkbox}\n";
        }

        checklistText.text = updatedText;
    }
}