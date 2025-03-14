using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    [SerializeField] private EquipmentType equipmentSlotType; // slotType �� equipmentSlotType �ɕύX

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
        // ���ݑI�𒆂̏ꍇ�A�c�[���`�b�v��\���i�}�E�X�̏ꍇ���Q�[���p�b�h�̏ꍇ�������ōX�V�����j
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

    // �������������������� ���z���\�b�h ��������������������
    /// <summary>
    /// �A�C�e���X���b�g�ɑ΂����{�̃A�N�V���������i�����Ȃǁj�B
    /// UI_EquipmentSlot �Ȃǂŏ㏑���\�B
    /// </summary>
    public virtual void HandleItemAction()
    {
        //Debug.Log("UI_ItemSlot: HandleItemAction called");
        // �K�v�ɉ����ċ��ʂ̏����������\
    }

    /// <summary>
    /// BaseEventData ���󂯎��ꍇ�̃n���h���B
    /// </summary>
    public virtual void HandleItemAction(BaseEventData eventData)
    {
        HandleItemAction();
    }
    // �������������������� �����܂� ��������������������

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

        // �C���F�A�C�e���̐����� ItemData.itemDescription ����擾
        itemDescription = itemData.itemDescription;

        SetButtonInteractable(true);

        // InventoryItem�t�B�[���h�̍X�V
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


    // �������������������� �Q�[���p�b�h����Ή� (�I����) ��������������������
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
