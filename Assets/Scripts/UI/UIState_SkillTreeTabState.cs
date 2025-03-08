using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIState_SkillTreeTabState : UIState
{
    private UIManager uiManager;
    private GameObject skillTreePanel;

    // Menu's Header 下にある "Skill Tree" ボタンのパス or 参照
    // ここでは最終的に transform.Find で取得する例を示す
    private const string MENU_HEADER_SKILL_TREE_PATH = "Menu's Header/Skill Tree";

    public UIState_SkillTreeTabState(
        UIStateMachine machine,
        UIManager uiManager,
        GameObject skillTreePanel,
        Button skillTreeHeaderButton // ← UIManager から受け取るが、使わない場合は削除してもOK
    )
        : base(machine)
    {
        this.uiManager = uiManager;
        this.skillTreePanel = skillTreePanel;
    }

    public override void Enter()
    {
        // 1) スキルツリーパネルをアクティブ化
        skillTreePanel.SetActive(true);
        uiManager.isSkillTreeActive = true;
        Debug.Log(
            "Enter SkillTreeTabState: skillTreePanel active = "
            + skillTreePanel.activeSelf
        );

        // 2) スキルツリー内のボタンを再設定＆有効化
        SetupSkillTreeButtons();
        EnableSkillTreeInteraction();

        // 3) ポップアップ初期化
        uiManager.skillUnlockPopup.SetActive(false);
        uiManager.isPopupActive = false;

        // 5) 次フレームに "Skill Tree" ボタンを選択するコルーチン
        uiManager.StartCoroutine(SelectDefaultButton());
    }

    private IEnumerator SelectDefaultButton()
    {
        // UI描画が完了するまで待機
        yield return new WaitForEndOfFrame();
        Debug.Log("SelectDefaultButton coroutine executed.");

        // ★ まず "Menu's Header/Skill Tree" ボタンを探す
        var skillTreeButtonTransform = skillTreePanel.transform.Find(MENU_HEADER_SKILL_TREE_PATH);
        if (skillTreeButtonTransform != null)
        {
            var skillTreeButton = skillTreeButtonTransform.GetComponent<Button>();
            if (skillTreeButton != null && skillTreeButton.gameObject.activeInHierarchy)
            {
                // ボタンをアクティブにする（必要であれば）
                skillTreeButton.gameObject.SetActive(true);

                // イベントシステムで選択
                EventSystem.current.SetSelectedGameObject(skillTreeButton.gameObject);
                Debug.Log("Selected 'Skill Tree' button from Menu's Header as default.");
                yield break;
            }
            else
            {
                Debug.LogWarning("'Skill Tree' Button found but not active or no Button component.");
            }
        }
        else
        {
            Debug.LogWarning("Could not find transform path: " + MENU_HEADER_SKILL_TREE_PATH);
        }

        // ★ 見つからない/アクティブでない場合、fallback でパネル内の先頭ボタンを選択
        var defaultButton = skillTreePanel.GetComponentInChildren<Button>(true);
        if (defaultButton != null && defaultButton.gameObject.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
            Debug.Log(
                "Selected default button from skillTreePanel: "
                + defaultButton.gameObject.name
            );
        }
        else
        {
            Debug.LogWarning("No active button found in skillTreePanel.");
        }
    }
    public override void Exit()
    {
        skillTreePanel.SetActive(false);
        uiManager.isSkillTreeActive = false;
        Debug.Log("Exit SkillTreeTabState");
        DisableSkillTreeInteraction();
    }


    public override void Update()
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject;
        Debug.Log(
            "UIState_SkillTreeTabState Update - Current selected: "
            + (currentSelected != null ? currentSelected.name : "null")
        );

        // Submitボタンが押されたら
        if (uiManager.submitAction != null && uiManager.submitAction.triggered)
        {
            if (!uiManager.isPopupActive)
            {
                HandleSkillTreeSubmit();
            }
            else
            {
                HandlePopupSubmit();
            }
        }
    }

    private void SetupSkillTreeButtons()
    {
        var skillSlots = skillTreePanel.GetComponentsInChildren<UI_SkillTreeSlot>(true);
        foreach (var slot in skillSlots)
        {
            var btn = slot.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    if (!uiManager.isPopupActive && !slot.unlocked)
                    {
                        ShowSkillPopup(slot);
                    }
                });
            }
        }
    }

    private void HandleSkillTreeSubmit()
    {
        var selectedObj = EventSystem.current.currentSelectedGameObject;
        if (selectedObj == null)
        {
            Debug.Log("HandleSkillTreeSubmit: No object selected.");
            return;
        }
        var skillSlot = selectedObj.GetComponent<UI_SkillTreeSlot>();
        if (skillSlot != null && !skillSlot.unlocked)
        {
            Debug.Log("HandleSkillTreeSubmit: Selected skill slot " + selectedObj.name);
            ShowSkillPopup(skillSlot);
        }
    }

    private void HandlePopupSubmit()
    {
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == uiManager.yesButton.gameObject)
        {
            UnlockSkill();
        }
        else if (selected == uiManager.noButton.gameObject)
        {
            CloseSkillPopup();
        }
    }

    public void ShowSkillPopup(UI_SkillTreeSlot skillSlot)
    {
        if (uiManager.isPopupActive) return;

        uiManager.currentSkillSlot = skillSlot;
        uiManager.skillUnlockPopup.SetActive(true);
        uiManager.isPopupActive = true;

        var cg = uiManager.skillUnlockPopup.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        uiManager.StartCoroutine(SelectYesButtonNextFrame());
    }

    private IEnumerator SelectYesButtonNextFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(uiManager.yesButton.gameObject);
        Debug.Log("SelectYesButtonNextFrame: Selected yesButton.");
    }

    private void UnlockSkill()
    {
        var skillSlot = uiManager.currentSkillSlot;
        if (skillSlot != null)
        {
            if (CurrencyManager.instance.HaveEnoughMoney(skillSlot.SkillCost))
            {
                CurrencyManager.instance.DeductMoney(skillSlot.SkillCost);
                skillSlot.unlocked = true;
                skillSlot.RefreshUI();
                uiManager.unlockedSkillSlot = skillSlot;
                Debug.Log("Skill unlocked: " + skillSlot.name);
            }
            else
            {
                Debug.LogWarning("Not enough souls to unlock the skill.");
            }
        }
        CloseSkillPopup();
    }

    private void CloseSkillPopup()
    {
        uiManager.skillUnlockPopup.SetActive(false);
        uiManager.isPopupActive = false;
        EnableSkillTreeInteraction();

        if (uiManager.currentSkillSlot != null)
        {
            var button = uiManager.currentSkillSlot.GetComponent<Button>();
            if (button != null && button.gameObject.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
                Debug.Log(
                    "CloseSkillPopup: Reselecting skill slot button: "
                    + button.gameObject.name
                );
            }
        }
        uiManager.unlockedSkillSlot = null;
        uiManager.currentSkillSlot = null;
    }

    private void EnableSkillTreeInteraction()
    {
        var buttons = skillTreePanel.GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
        {
            button.interactable = true;
        }
    }

    private void DisableSkillTreeInteraction()
    {
        var buttons = skillTreePanel.GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
    }
}
