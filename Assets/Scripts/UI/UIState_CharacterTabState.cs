using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIState_CharacterTabState : UIState
{
    private UIManager uiManager;
    private GameObject characterPanel;

    public UIState_CharacterTabState(UIStateMachine machine, UIManager uiManager, GameObject characterPanel)
        : base(machine)
    {
        this.uiManager = uiManager;
        this.characterPanel = characterPanel;
    }

    public override void Enter()
    {
        characterPanel.SetActive(true);
        uiManager.isCharacterActive = true;
        Debug.Log("Enter CharacterTabState");

        // キャラクタータブボタンにフォーカスを与える
        if (uiManager.characterTabButton != null)
        {
            EventSystem.current.SetSelectedGameObject(uiManager.characterTabButton.gameObject);
        }
        else
        {
            var firstButton = characterPanel.GetComponentInChildren<Button>();
            if (firstButton != null)
                EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }
    }

    public override void Exit()
    {
        characterPanel.SetActive(false);
        uiManager.isCharacterActive = false;
        Debug.Log("Exit CharacterTabState");
    }

    public override void Update()
    {
        // Submitボタンが押されたら、現在の選択オブジェクトに応じた処理を実行
        if (uiManager.submitAction != null && uiManager.submitAction.triggered)
        {
            HandleCharacterSubmit();
        }
    }

    private void HandleCharacterSubmit()
    {
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
        Debug.Log("[HandleCharacterSubmit] Selected object: " + (selectedObj != null ? selectedObj.name : "null"));
        if (selectedObj == null)
        {
            Debug.LogWarning("[HandleCharacterSubmit] No selected object found.");
            return;
        }

        //// EquipmentSelectionPanel が開いている場合、CloseButtonが選択されているなら閉じる
        //if (uiManager.equipmentSelectionPanel != null &&
        //    selectedObj == uiManager.equipmentSelectionPanel.closeEquipmentSelectionPanel.gameObject)
        //{
        //    Debug.Log("[HandleCharacterSubmit] CloseButton pressed, closing EquipmentSelectionPanel.");
        //    uiManager.equipmentSelectionPanel.OnCloseButton();
        //    return;
        //}

        //EquipmentSelectionPanel が開いている場合、CloseButtonが選択されているなら閉じる
        //if (uiManager.skillSelectionPanel != null &&
        //    selectedObj == uiManager.skillSelectionPanel.closeSkillSelectionPanel.gameObject)
        //{
        //    Debug.Log("[HandleCharacterSubmit] CloseButton pressed, closing SkillSelectionPanel.");
        //    uiManager.skillSelectionPanel.OnCloseButton();
        //    return;
        //}

        // UI_EquipmentSetSlot が選択されているかチェック
        UI_EquipmentSetSlot eqSlot = selectedObj.GetComponent<UI_EquipmentSetSlot>();
        if (eqSlot != null)
        {
            Debug.Log("[HandleCharacterSubmit] UI_EquipmentSlot component found on " + selectedObj.name);
            uiManager.OpenEquipmentSelectionPanel(eqSlot.slotType, eqSlot);
            return;
        }

        // UI_ItemSlot は EquipmentSelectionPanel 内で処理されるので、ここでは無視
        UI_ItemSlot itemSlot = selectedObj.GetComponent<UI_ItemSlot>();
        if (itemSlot != null)
        {
            Debug.LogWarning("[HandleCharacterSubmit] Selected object '" + selectedObj.name + "' is a UI_ItemSlot. This should be handled by the EquipmentSelectionPanel.");
            return;
        }

        Debug.LogWarning("[HandleCharacterSubmit] Selected object '" + selectedObj.name + "' is neither a UI_EquipmentSlot nor a UI_ItemSlot.");
    }



}
