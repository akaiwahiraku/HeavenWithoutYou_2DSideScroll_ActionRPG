using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSetSlot : UI_ItemSlot
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = "Equipment slot - " + slotType.ToString();
    }

    // UI_ItemSlot �� Update() ���B�����A�c�[���`�b�v�\���𖳌�������
    private new void Update()
    {
        // EquipmentSetSlot �ł̓c�[���`�b�v��\�����Ȃ�
        // ���K�v�Ȃ�A�����ŕʂ̏����i�Ⴆ�ΑI����Ԃ̊Ǘ��Ȃǁj�������ł��܂�
    }

    public override void HandleItemAction()
    {
        Debug.Log("EquipmentSlot: HandleItemAction called without BaseEventData.");
        if (item == null || item.data == null)
            return;

        // �������A�C�e���̉����ƃC���x���g���ւ̖߂������s
        Inventory.instance.UnequipItem(item.data as ItemData_Equipment);
        Inventory.instance.AddItem(item.data as ItemData_Equipment);

        uiManager.itemToolTip.HideToolTip();
        CleanUpSlot();
    }

    public override void HandleItemAction(BaseEventData eventData)
    {
        // ���ʏ����Ƃ��Ĉ����Ȃ��ł��Ăяo��
        HandleItemAction();
    }
}
