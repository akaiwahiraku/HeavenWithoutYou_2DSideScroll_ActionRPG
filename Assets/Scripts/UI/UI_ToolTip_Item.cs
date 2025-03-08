using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ToolTip_Item : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private float defaultFontSize = 28;

    private void Start()
    {
        var graphics = GetComponentsInChildren<UnityEngine.UI.Graphic>(true);
        foreach (var g in graphics)
        {
            g.raycastTarget = false;
        }
    }

    public void ShowToolTip(string _itemName, string _itemType, string _itemDescription)
    {
        itemName.text = _itemName;
        itemType.text = _itemType;
        itemDescription.text = _itemDescription;

        //AdjustPosition();
        //AdjustFontSize(skillName);

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        itemName.fontSize = defaultFontSize;
        gameObject.SetActive(false);
    }
}
