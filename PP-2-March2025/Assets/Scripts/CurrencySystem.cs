using UnityEngine;
using UnityEngine.Events;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem instance;

    public UnityEvent<int> OnMoneyChanged;
    [SerializeField] private int currentMoney;
    public int CurrentMoney => currentMoney;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            OnMoneyChanged?.Invoke(currentMoney);
            return true;
        }
        return false;
    }
}
