using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class UI_ItemSlot : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemAmountText;
    [SerializeField] protected TextMeshProUGUI itemNameText;

    [SerializeField] private string itemName;
    [SerializeField] private string itemType;
    [TextArea]
    [SerializeField] private string itemDescription;

    protected UIManager uiManager;
    public InventoryItem item;

    private Button button;
    [SerializeField] private EquipmentType equipmentSlotType; // slotType を equipmentSlotType に変更

    private void Awake()
    {
        uiManager = GetComponentInParent<UIManager>();
    }

    protected virtual void Start()
    {
        button = GetComponent<Button>();
        uiManager.itemToolTip.HideToolTip();
    }

    private void Update()
    {
        // 現在選択中の場合、ツールチップを表示（マウスの場合もゲームパッドの場合もここで更新される）
        bool isCurrentlySelected = EventSystem.current.currentSelectedGameObject == gameObject;
        if (isCurrentlySelected)
        {
            //Debug.Log($"ItemSlot {gameObject.name}: Showing tooltip with Name={itemName}, Type={itemType}, Description={itemDescription}");
            uiManager.itemToolTip.ShowToolTip(itemName, itemType, itemDescription);
        }
    }

    private void OnValidate()
    {
        gameObject.name = "Equipment slot - " + equipmentSlotType.ToString();
    }

    private void OnDisable()
    {
        if (uiManager != null && uiManager.itemToolTip != null)
        {
            uiManager.itemToolTip.HideToolTip();
        }
    }

    // ────────── 仮想メソッド ──────────
    /// <summary>
    /// アイテムスロットに対する基本のアクション処理（装備など）。
    /// UI_EquipmentSlot などで上書き可能。
    /// </summary>
    public virtual void HandleItemAction()
    {
        //Debug.Log("UI_ItemSlot: HandleItemAction called");
        // 必要に応じて共通の処理を実装可能
    }

    /// <summary>
    /// BaseEventData を受け取る場合のハンドラ。
    /// </summary>
    public virtual void HandleItemAction(BaseEventData eventData)
    {
        HandleItemAction();
    }
    // ────────── ここまで ──────────

    public void UpdateSlot(ItemData itemData, int stackSize)
    {
        if (itemData == null)
        {
            //Debug.LogWarning("[UpdateSlot] Null itemData for slot: " + gameObject.name);
            CleanUpSlot();
            SetButtonInteractable(false);
            return;
        }

        //Debug.Log("[UpdateSlot] Setting slot " + gameObject.name + " with item: " + itemData.itemName + " (stack: " + stackSize + ")");

        if (itemImage == null || itemAmountText == null || itemNameText == null)
        {
            //Debug.LogError("[UpdateSlot] UI components not properly assigned in " + gameObject.name);
            SetButtonInteractable(false);
            return;
        }

        itemImage.sprite = itemData.itemIcon;
        itemImage.color = Color.white;
        itemAmountText.text = stackSize > 1 ? stackSize.ToString() : "";
        itemNameText.text = itemData.itemName;

        // 修正：アイテムの説明は ItemData.itemDescription から取得
        itemDescription = itemData.itemDescription;

        SetButtonInteractable(true);

        // InventoryItemフィールドの更新
        this.item = new InventoryItem(itemData);

        //Debug.Log("[UpdateSlot] Slot " + gameObject.name + " updated successfully.");
    }


    public virtual void CleanUpSlot()
    {
        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.color = Color.clear;
        }
        if (itemAmountText != null)
        {
            itemAmountText.text = "";
        }
        if (itemNameText != null)
        {
            itemNameText.text = "";
        }
    }

    private void SetButtonInteractable(bool isActive)
    {
        if (button != null)
        {
            button.interactable = isActive;
        }
    }


    // ────────── ゲームパッド操作対応 (選択時) ──────────
    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log("[UI_ItemSlot] OnSelect called on: " + gameObject.name);
        if (item != null && item.data != null && item.data.itemType == ItemType.Equipment)
        {
            UIManager uiManager = GetComponentInParent<UIManager>();
            if (uiManager != null)
            {
                //Debug.Log("[UI_ItemSlot] (Select) Calling ShowStatPreview for item: " + item.data.itemName);
                uiManager.ShowStatPreview(item.data as ItemData_Equipment);
            }
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Debug.Log("[UI_ItemSlot] OnDeselect called on: " + gameObject.name);
        UIManager uiManager = GetComponentInParent<UIManager>();
        if (uiManager != null)
        {
            uiManager.ClearStatPreview();
        }
    }
}
