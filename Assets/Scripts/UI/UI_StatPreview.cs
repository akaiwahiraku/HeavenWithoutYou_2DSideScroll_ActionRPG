using TMPro;
using UnityEngine;

public class UI_StatPreview : MonoBehaviour
{
    [Header("Stat Preview Panel")]
    [SerializeField] private GameObject statPreviewPanel;
    [SerializeField] private TextMeshProUGUI strengthPreviewText;
    [SerializeField] private TextMeshProUGUI agilityPreviewText;
    [SerializeField] private TextMeshProUGUI intelligencePreviewText;
    [SerializeField] private TextMeshProUGUI vitalityPreviewText;
    [SerializeField] private TextMeshProUGUI damagePreviewText;
    [SerializeField] private TextMeshProUGUI critChancePreviewText;
    [SerializeField] private TextMeshProUGUI critPowerPreviewText;
    [SerializeField] private TextMeshProUGUI armorPreviewText;
    [SerializeField] private TextMeshProUGUI evasionPreviewText;
    [SerializeField] private TextMeshProUGUI magicResistancePreviewText;
    [SerializeField] private TextMeshProUGUI fireDamagePreviewText;
    [SerializeField] private TextMeshProUGUI iceDamagePreviewText;
    [SerializeField] private TextMeshProUGUI lightningDamagePreviewText;

    /// <summary>
    /// 装備のステータス差分を計算し、プレビュー用UIに反映します。
    /// </summary>
    public void ShowStatPreview(ItemData_Equipment hoveredEquipment)
    {
        if (hoveredEquipment == null)
            return;

        // 現在装備しているアイテムを取得（Inventory はシングルトンで管理している前提）
        ItemData_Equipment currentEquipment = Inventory.instance.GetEquipment(hoveredEquipment.equipmentType);

        int diffStrength = hoveredEquipment.strength - (currentEquipment != null ? currentEquipment.strength : 0);
        int diffAgility = hoveredEquipment.agility - (currentEquipment != null ? currentEquipment.agility : 0);
        int diffIntelligence = hoveredEquipment.intelligence - (currentEquipment != null ? currentEquipment.intelligence : 0);
        int diffVitality = hoveredEquipment.vitality - (currentEquipment != null ? currentEquipment.vitality : 0);

        int diffDamage = (hoveredEquipment.damage + hoveredEquipment.strength)
                         - (currentEquipment != null ? (currentEquipment.damage + currentEquipment.strength) : 0);
        int diffCritChance = (hoveredEquipment.critChance + hoveredEquipment.agility)
                             - (currentEquipment != null ? (currentEquipment.critChance + currentEquipment.agility) : 0);
        int diffCritPower = (hoveredEquipment.critPower + hoveredEquipment.strength)
                            - (currentEquipment != null ? (currentEquipment.critPower + currentEquipment.strength) : 0);

        int diffArmor = hoveredEquipment.armor - (currentEquipment != null ? currentEquipment.armor : 0);
        int diffEvasion = (hoveredEquipment.evasion + hoveredEquipment.agility)
                          - (currentEquipment != null ? (currentEquipment.evasion + currentEquipment.agility) : 0);
        int diffMagicRes = (hoveredEquipment.magicResistance + hoveredEquipment.intelligence * 3)
                           - (currentEquipment != null ? (currentEquipment.magicResistance + currentEquipment.intelligence * 3) : 0);

        int diffFireDamage = hoveredEquipment.fireDamage - (currentEquipment != null ? currentEquipment.fireDamage : 0);
        int diffIceDamage = hoveredEquipment.iceDamage - (currentEquipment != null ? currentEquipment.iceDamage : 0);
        int diffLightningDamage = hoveredEquipment.lightningDamage - (currentEquipment != null ? currentEquipment.lightningDamage : 0);

        strengthPreviewText.text = FormatDiff(diffStrength);
        agilityPreviewText.text = FormatDiff(diffAgility);
        intelligencePreviewText.text = FormatDiff(diffIntelligence);
        vitalityPreviewText.text = FormatDiff(diffVitality);

        damagePreviewText.text = FormatDiff(diffDamage);
        critChancePreviewText.text = FormatDiff(diffCritChance);
        critPowerPreviewText.text = FormatDiff(diffCritPower);

        armorPreviewText.text = FormatDiff(diffArmor);
        evasionPreviewText.text = FormatDiff(diffEvasion);
        magicResistancePreviewText.text = FormatDiff(diffMagicRes);

        fireDamagePreviewText.text = FormatDiff(diffFireDamage);
        iceDamagePreviewText.text = FormatDiff(diffIceDamage);
        lightningDamagePreviewText.text = FormatDiff(diffLightningDamage);

        statPreviewPanel.SetActive(true);
    }

    /// <summary>
    /// 数値の差分に応じたフォーマット文字列を返します。
    /// </summary>
    private string FormatDiff(int diff)
    {
        if (diff == 0)
            return "";
        string color = diff > 0 ? "green" : "red";
        string sign = diff > 0 ? "+" : "";
        return $"<color={color}>{sign}{diff}</color>";
    }

    /// <summary>
    /// ステータスプレビューを非表示にします。
    /// </summary>
    public void ClearStatPreview()
    {
        statPreviewPanel.SetActive(false);
    }
}
