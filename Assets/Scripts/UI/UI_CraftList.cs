using System.Collections.Generic;
using UnityEngine;

public class UI_CraftList : MonoBehaviour
{
    [SerializeField] private Transform craftSlotParent;
    [SerializeField] private GameObject craftSlotPrefab;
    [SerializeField] private List<ItemData_Equipment> craftEquipment;

    void Start()
    {
        if (craftEquipment == null || craftEquipment.Count == 0)
        {
            Debug.LogWarning("Craft equipment list is empty or null.");
            return;
        }

        SetupCraftList();
        SetupDefaultCraftWindow();
    }

    public void SetupCraftList()
    {
        // 子オブジェクトをすべて削除
        for (int i = 0; i < craftSlotParent.childCount; i++)
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }

        // スロットを生成
        foreach (var equipment in craftEquipment)
        {
            if (equipment != null)
            {
                GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);
                var craftSlot = newSlot.GetComponent<UI_CraftSlot>();
                craftSlot.SetupCraftSlot(equipment); // データを設定
            }
        }
    }

    public void SetupDefaultCraftWindow()
    {
        if (craftEquipment != null && craftEquipment.Count > 0 && craftEquipment[0] != null)
        {
            GetComponentInParent<UIManager>().craftWindow.SetupCraftWindow(craftEquipment[0]);
        }
    }
}
