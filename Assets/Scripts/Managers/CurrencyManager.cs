using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour, ISaveManager
{
    public static CurrencyManager instance;
    public Player player;

    public int currency; // privateで保持

    public int Currency
    {
        get { return currency; }
        set
        {
            currency = value;
            OnCurrencyChanged?.Invoke(currency); // ソウル数の変更時にイベント発火
        }
    }

    public event Action<int> OnCurrencyChanged;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    public bool HaveEnoughMoney(int _price)
    {
        if (_price > currency)
        {
            Debug.Log("Not enough money");
            return false;
        }

        Currency -= _price;
        return true;
    }

    public void DeductMoney(int amount)
    {
        if (HaveEnoughMoney(amount))
        {
            currency -= amount;
            Debug.Log($"Souls deducted: {amount}. Remaining souls: {currency}");
        }
        else
        {
            Debug.LogWarning("Not enough souls!");
        }
    }

    public int GetCurrency() => currency;

    public void LoadData(GameData _data)
    {
        Currency = _data.currency;
    }

    public void SaveData(ref GameData _data)
    {
        _data.currency = Currency;
    }
}
