using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{
    private ItemData_Equipment currentData; // 現在のスロットデータを保存

    protected override void Start()
    {
        base.Start();
    }

    public void SetupCraftSlot(ItemData_Equipment _data)
    {
        if (_data == null)
        {
            CleanUpSlot(); // データがない場合はスロットを初期化
            return;
        }

        currentData = _data; // データを保存
        itemImage.sprite = _data.itemIcon; // アイコンを設定
        itemImage.color = Color.white; // 色をリセット
        //itemAmountText.text = _data.itemName; // アイテム名を表示
        itemNameText.text = _data.itemName; // アイテム名を表示

        // アイテム名が長い場合にフォントサイズを調整
        //itemAmountText.fontSize = _data.itemName.Length > 12 ? itemAmountText.fontSize * 0.7f : 24;
        itemNameText.fontSize = _data.itemName.Length > 12 ? itemNameText.fontSize * 0.7f : 24;
    }

    //public override void OnPointerDown(PointerEventData eventData)
    public void OnSelect(BaseEventData eventData)
    {
        if (currentData != null)
        {
            uiManager.craftWindow.SetupCraftWindow(currentData);
        }
    }

    public override void CleanUpSlot()
    {
        currentData = null; // データをクリア
        itemImage.sprite = null; // アイコンをリセット
        itemImage.color = Color.clear; // 色をクリア
        //itemAmountText.text = ""; // テキストをクリア
        itemNameText.text = ""; // テキストをクリア
    }
}
