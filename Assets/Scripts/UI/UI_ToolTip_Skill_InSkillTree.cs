using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ToolTip_Skill_InSkillTree : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillCost;
    [SerializeField] private float defaultNameFontSize;

    // 例: ツールチップ表示時にRaycastTargetを無効化
    private void Start()
    {
        var graphics = GetComponentsInChildren<UnityEngine.UI.Graphic>(true);
        foreach (var g in graphics)
        {
            g.raycastTarget = false;
        }
    }

    public void ShowToolTip(string _skillName, string _skillDescription, int _price)
    {
        skillName.text = _skillName;
        skillDescription.text = _skillDescription;
        skillCost.text = "Cost: " + _price;

        //AdjustPosition();
        //AdjustFontSize(skillName);

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        skillName.fontSize = defaultNameFontSize;
        gameObject.SetActive(false);
    }
}
