using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSetSlot : UI_ItemSlot
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = "Equipment slot - " + slotType.ToString();
    }

    // UI_ItemSlot の Update() を隠蔽し、ツールチップ表示を無効化する
    private new void Update()
    {
        // EquipmentSetSlot ではツールチップを表示しない
        // ※必要なら、ここで別の処理（例えば選択状態の管理など）を実装できます
    }

    public override void HandleItemAction()
    {
        Debug.Log("EquipmentSlot: HandleItemAction called without BaseEventData.");
        if (item == null || item.data == null)
            return;

        // 装備中アイテムの解除とインベントリへの戻しを実行
        Inventory.instance.UnequipItem(item.data as ItemData_Equipment);
        Inventory.instance.AddItem(item.data as ItemData_Equipment);

        uiManager.itemToolTip.HideToolTip();
        CleanUpSlot();
    }

    public override void HandleItemAction(BaseEventData eventData)
    {
        // 共通処理として引数なし版を呼び出す
        HandleItemAction();
    }
}
