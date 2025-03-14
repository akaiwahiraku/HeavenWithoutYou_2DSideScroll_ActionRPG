using TMPro;
using UnityEngine;

public class UI_ToolTip_Skill_InCharacter : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private float defaultNameFontSize = 28;

    // 例: ツールチップ表示時にRaycastTargetを無効化
    private void Start()
    {
        var graphics = GetComponentsInChildren<UnityEngine.UI.Graphic>(true);
        foreach (var g in graphics)
        {
            g.raycastTarget = false;
        }
    }

    public void ShowToolTip(string _skillName, string _skillDescription)
    {
        skillName.text = _skillName;
        skillDescription.text = _skillDescription;

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
