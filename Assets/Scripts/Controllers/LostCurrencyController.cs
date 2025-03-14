using UnityEngine;

public class LostCurrencyController : MonoBehaviour
{
    public int currency;

    // LostCurrencyController.cs
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            CurrencyManager.instance.Currency += currency; // Currencyプロパティを介してアクセス
            Destroy(this.gameObject);
        }
    }

}
