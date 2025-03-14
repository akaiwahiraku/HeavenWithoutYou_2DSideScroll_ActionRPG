using TMPro;
using UnityEngine;

public class UI_StatSlot : MonoBehaviour
{
    private UIManager ui;

    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;

    private void OnValidate()
    {
        gameObject.name = "Stat - " + statName;

        if (statValueText != null)
            statNameText.text = statName;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateStatValueUI();

        ui = GetComponentInParent<UIManager>();
    }

    void Update()
    {
        UpdateStatValueUI();
    }



    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = CurrencyManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            statValueText.text = playerStats.GetStat(statType).GetValue().ToString();

            if (statType == StatType.strength)
                playerStats.strength.GetValue().ToString();

            if (statType == StatType.agility)
                playerStats.agility.GetValue().ToString();

            if (statType == StatType.intelligence)
                playerStats.intelligence.GetValue().ToString();

            if (statType == StatType.vitality)
                playerStats.vitality.GetValue().ToString();

            if (statType == StatType.health)
                statValueText.text = playerStats.GetMaxHealthValue().ToString();

            if (statType == StatType.damage)
                statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();

            if (statType == StatType.critPower)
                statValueText.text = (playerStats.critPower.GetValue() + playerStats.strength.GetValue()).ToString();

            if (statType == StatType.critChance)
                statValueText.text = (playerStats.critChance.GetValue() + playerStats.agility.GetValue()).ToString();

            if (statType == StatType.evasion)
                statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();

            if (statType == StatType.magicRes)
                statValueText.text = (playerStats.magicResistance.GetValue() + (playerStats.intelligence.GetValue() * 3)).ToString();
        }
    }
}
