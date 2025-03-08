using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{
    private ItemData_Equipment currentData; // ���݂̃X���b�g�f�[�^��ۑ�

    protected override void Start()
    {
        base.Start();
    }

    public void SetupCraftSlot(ItemData_Equipment _data)
    {
        if (_data == null)
        {
            CleanUpSlot(); // �f�[�^���Ȃ��ꍇ�̓X���b�g��������
            return;
        }

        currentData = _data; // �f�[�^��ۑ�
        itemImage.sprite = _data.itemIcon; // �A�C�R����ݒ�
        itemImage.color = Color.white; // �F�����Z�b�g
        //itemAmountText.text = _data.itemName; // �A�C�e������\��
        itemNameText.text = _data.itemName; // �A�C�e������\��

        // �A�C�e�����������ꍇ�Ƀt�H���g�T�C�Y�𒲐�
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
        currentData = null; // �f�[�^���N���A
        itemImage.sprite = null; // �A�C�R�������Z�b�g
        itemImage.color = Color.clear; // �F���N���A
        //itemAmountText.text = ""; // �e�L�X�g���N���A
        itemNameText.text = ""; // �e�L�X�g���N���A
    }
}
