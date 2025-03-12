using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Image reloadCircle;

    public void UpdateAmmo(int current, int max)
    {
        ammoText.text = $"Ammo: {current} / {max}";
    }

    public void UpdateReloadProgress(float percent)
    {
        if (reloadCircle != null)
            reloadCircle.fillAmount = percent;
    }

    public void Show(bool visible)
    {
        gameObject.SetActive(visible);
    }
}