using TMPro;
using UnityEngine;


public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    void Start()
    {
        CurrencySystem.instance.OnMoneyChanged.AddListener(UpdateMoneyText);
        UpdateMoneyText(CurrencySystem.instance.CurrentMoney);
    }

    void UpdateMoneyText(int money)
    {
        moneyText.text = "$" + money.ToString();
    }
}
