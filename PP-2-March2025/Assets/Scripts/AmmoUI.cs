using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text ammoText;

    public void UpdateAmmo(int current, int max)
    {
        ammoText.text = $"Ammo: {current} / {max}";
    }
}