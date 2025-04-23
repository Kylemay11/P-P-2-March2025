using TMPro;
using UnityEngine;

public class WinStatsUI : MonoBehaviour
{
    public TMP_Text zombiesKilledText;
    public TMP_Text moneyEarnedText;
    public TMP_Text wavesClearedText;

    public void ShowStats(int zombiesKilled, int moneyEarned, int wavesCleared)
    {
        zombiesKilledText.text = $"Zombies Killed: {zombiesKilled}";
        moneyEarnedText.text = $"Money Earned: ${moneyEarned}";
        wavesClearedText.text = $"Waves Cleared: {wavesCleared}";
    }
}