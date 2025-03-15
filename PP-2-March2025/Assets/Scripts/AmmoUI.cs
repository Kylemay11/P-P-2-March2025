using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    public static AmmoUI instance;

    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Image reloadCircle;

    private bool isReloading = false;
    private float reloadTime = 0f;
    private float reloadTimer = 0f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Update()
    {
        if (isReloading)
        {
            reloadTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(reloadTimer / reloadTime);
            UpdateReloadProgress(progress);

            if (progress >= 1f)
            {
                isReloading = false;
                ShowReloadIndicator(false);
            }
        }
    }


    public void UpdateAmmo(int current, int max)
    {
        if (ammoText != null)
            ammoText.text = $"{current} / {max}";
    }

    public void StartReload(float time)
    {
        if (reloadCircle == null) return;

        reloadTime = time;
        reloadTimer = 0f;
        isReloading = true;
        ShowReloadIndicator(true);
        UpdateReloadProgress(0f);
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

    private void ShowReloadIndicator(bool show)
    {
        if (reloadCircle != null)
            reloadCircle.gameObject.SetActive(show);
    }
}