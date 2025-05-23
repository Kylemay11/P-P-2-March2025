using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    public static AmmoUI instance;

    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text throwableText;
    [SerializeField] private TMP_Text emptyActionText;
    [SerializeField] private Image reloadCircle;

    private bool isReloading = false;
    private float reloadTime;
    private float reloadTimer;

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
            ammoText.text = "#" + (playerController.instance.wepListPos+1) + " " + $"{current} / {max}";
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

    // Jacob added
    public void StopReload()
    {
        isReloading = false;
        if (reloadCircle != null)
        {
            reloadCircle.fillAmount = 0f;
        }
        UpdateReloadProgress(0f);
        ShowReloadIndicator(false);

        gameObject.SetActive(false);

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
    public void UpdateThrowable(int current, int max)
    {
        if (throwableText != null)
            throwableText.text = "Q " + $"{current} / {max}";
    }

    public void ThrowablesEmpty()
    {
        if (emptyActionText) // !null
            StartCoroutine(DisplayEmptyText());
    }

    private IEnumerator DisplayEmptyText()
    {
        emptyActionText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.3f);

        emptyActionText.gameObject.SetActive(false);
    }
}