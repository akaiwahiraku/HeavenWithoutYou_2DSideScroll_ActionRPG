using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_EquipmentSelectionPanel : MonoBehaviour
{
    [SerializeField] public Transform equipmentListContainer;
    [SerializeField] private GameObject equipmentButtonPrefab;

    private EquipmentType currentEquipmentType;
    private UI_EquipmentSetSlot currentSlot;

    // 候補の初期選択対象として保持するオブジェクト
    private GameObject firstOption;

    /// <summary>
    /// 装備選択パネルを開く。指定された EquipmentType に合致するインベントリアイテムのみ表示する。
    /// </summary>
    public void Open(EquipmentType equipmentType, UI_EquipmentSetSlot slot)
    {
        currentEquipmentType = equipmentType;
        currentSlot = slot;

        gameObject.SetActive(true);
        PopulateEquipmentList();
        StartCoroutine(DelayedSetInitialFocus());
    }

    /// <summary>
    /// 遅延して初期選択対象を設定する。
    /// 候補がある場合はその最初のオブジェクト、なければ currentSlot を設定
    /// </summary>
    private IEnumerator DelayedSetInitialFocus()
    {
        yield return null; // 1フレーム待機

        if (equipmentListContainer.childCount > 0)
        {
            firstOption = equipmentListContainer.GetChild(0).gameObject;
            EventSystem.current.SetSelectedGameObject(firstOption);
        }
        else if (currentSlot != null)
        {
            firstOption = currentSlot.gameObject;
            EventSystem.current.SetSelectedGameObject(firstOption);
        }
    }

    /// <summary>
    /// Update 内で、パネル外の選択を検出し、強制的に firstOption に戻す
    /// </summary>
    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
        if (firstOption == null) return;

        bool isInPanel = false;

        // equipmentListContainer 内の候補ボタン内にあるかをチェック
        foreach (Transform child in equipmentListContainer)
        {
            if (currentSelected == child.gameObject || (currentSelected != null && currentSelected.transform.IsChildOf(child)))
            {
                isInPanel = true;
                break;
            }
        }

        // currentSlot もパネル内の要素とみなす
        if (currentSelected == currentSlot.gameObject)
        {
            isInPanel = true;
        }

        // パネル外の選択になっている場合、強制的に firstOption に戻す
        if (!isInPanel && firstOption.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(firstOption);
        }
    }

    /// <summary>
    /// Inventory の中から、currentEquipmentType に合致する装備アイテムをボタンとして生成する。
    /// </summary>
    private void PopulateEquipmentList()
    {
        // 既存のボタンを削除
        foreach (Transform child in equipmentListContainer)
        {
            Destroy(child.gameObject);
        }

        List<InventoryItem> filteredItems = new List<InventoryItem>();
        // Inventory.instance.inventoryDictionary から装備アイテムかつ該当カテゴリーのみ抽出
        foreach (var pair in Inventory.instance.inventoryDictionary)
        {
            if (pair.Key is ItemData_Equipment equip && equip.equipmentType == currentEquipmentType)
            {
                filteredItems.Add(pair.Value);
            }
        }

        if (filteredItems.Count == 0)
        {
            Debug.LogWarning("No equipment items available for type: " + currentEquipmentType);
            // 該当アイテムが無い場合は、currentSlot にフォーカスを戻し、パネルを閉じる
            if (currentSlot != null)
            {
                firstOption = currentSlot.gameObject;
                StartCoroutine(CloseAndReturnFocus());
            }
            return;
        }

        // ボタン生成
        foreach (var invItem in filteredItems)
        {
            GameObject btnObj = Instantiate(equipmentButtonPrefab, equipmentListContainer);
            Button btn = btnObj.GetComponent<Button>();
            TextMeshProUGUI label = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null && invItem.data != null)
            {
                label.text = invItem.data.itemName;
            }
            btn.onClick.AddListener(() => OnEquipmentSelected(invItem.data as ItemData_Equipment));

            // イベントトリガーを追加してプレビュー処理を実装
            EventTrigger trigger = btnObj.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = btnObj.AddComponent<EventTrigger>();
            }

            // 「Select」イベント：選択時にツールチップ＆ステータスプレビューを表示
            EventTrigger.Entry selectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Select };
            selectEntry.callback.AddListener((data) =>
            {
                if (UIManager.instance != null && invItem.data != null)
                {
                    // 修正：ItemDataに記述された itemDescription をそのまま使用
                    UIManager.instance.itemToolTip.ShowToolTip(
                        invItem.data.itemName,
                        "Equipment",
                        invItem.data.itemDescription
                    );
                    UIManager.instance.ShowStatPreview(invItem.data as ItemData_Equipment);
                }
            });
            trigger.triggers.Add(selectEntry);

            // 「Deselect」イベント：選択解除時にツールチップ＆プレビューを非表示
            EventTrigger.Entry deselectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Deselect };
            deselectEntry.callback.AddListener((data) =>
            {
                if (UIManager.instance != null)
                {
                    UIManager.instance.itemToolTip.HideToolTip();
                    UIManager.instance.ClearStatPreview();
                }
            });
            trigger.triggers.Add(deselectEntry);
        }

    }

    /// <summary>
    /// 選択された装備アイテムを適用する。
    /// </summary>
    private void OnEquipmentSelected(ItemData_Equipment selectedEquipment)
    {
        if (selectedEquipment == null)
        {
            Debug.LogWarning("Selected equipment is null.");
            return;
        }
        // 装備変更（古い装備は EquipItem 内で自動的にインベントリへ戻る）
        Inventory.instance.EquipItem(selectedEquipment);
        Close();
    }

    /// <summary>
    /// パネルを閉じ、currentSlot にフォーカスを戻す
    /// </summary>
    private void Close()
    {
        StartCoroutine(CloseAndReturnFocus());
    }

    private IEnumerator CloseAndReturnFocus()
    {
        yield return null; // 1フレーム待機

        if (currentSlot != null)
        {
            Button btn = currentSlot.GetComponent<Button>();
            if (btn != null)
            {
                btn.Select();
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(currentSlot.gameObject);
            }
        }
        yield return null; // さらに1フレーム待機

        gameObject.SetActive(false);
        Debug.Log("[CloseAndReturnFocus] EquipmentSelectionPanel deactivated.");
    }
}
