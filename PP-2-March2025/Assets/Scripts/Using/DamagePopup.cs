using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float floatSpeed;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float destroyDelay;

    private TextMeshProUGUI text;
    private Color startColor;
    private float timer;
    private bool isInitialized = false;

    public void Setup(float damage)
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        if (text == null)
        {
            Debug.LogWarning("DamagePopup: No TextMeshProUGUI found.");
            return;
        }

        text.text = damage.ToString("F0");
        startColor = text.color;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer > destroyDelay)
        {
            startColor.a -= fadeSpeed * Time.deltaTime;
            text.color = startColor;

            if (startColor.a <= 0f)
                Destroy(gameObject);
        }
    }
}