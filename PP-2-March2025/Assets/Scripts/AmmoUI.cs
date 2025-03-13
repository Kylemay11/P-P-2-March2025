using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    public static AmmoUI instance;

    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Image reloadCircle;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void UpdateAmmo(int current, int max)
    {
        if (ammoText != null)
            ammoText.text = $"{current} / {max}";
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