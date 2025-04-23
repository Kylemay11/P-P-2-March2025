using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponNotificationUI : MonoBehaviour
{
    [SerializeField] private TMP_Text weaponText;
    [SerializeField] private float displayDuration;
    [SerializeField] private float fadeSpeed;

    private float timer;
    [SerializeField]private CanvasGroup canvasGroup;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (canvasGroup.alpha > 0)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, fadeSpeed * Time.deltaTime);
            }
        }
    }

    public void ShowWeaponName(string name)
    {
        weaponText.text = name;
        timer = displayDuration;
        canvasGroup.alpha = 1f;
    }
}