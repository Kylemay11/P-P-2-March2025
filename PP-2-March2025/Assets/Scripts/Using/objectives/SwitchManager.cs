using UnityEngine;

public class SwitchManager : MonoBehaviour
{
    public DoorInteract eventDoor;  // Assign the event door you want to open
    public int totalSwitches;    // How many switches needed
    private int flippedSwitches = 0;

    public void SwitchFlipped()
    {
        flippedSwitches++;

        if (flippedSwitches >= totalSwitches)
        {
            eventDoor.CompleteEvent();
        }
    }
}