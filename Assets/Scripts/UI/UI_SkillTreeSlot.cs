using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour, ISaveManager
{
    private UIManager uiManager;
    private UIState_SkillTreeTabState uiSkillTreeTabState;

    private Image skillImage;
    [SerializeField] private int skillCost;
    public int SkillCost { get; private set; } = 100; // 仮のコスト値

    // 旧来の表示用フィールド
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;

    [SerializeField] private Color lockedSkillColor;
    [SerializeField] private Color highlightColor = new Color(0, 1, 1, 130f / 255f);

    [SerializeField] public UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] public UI_SkillTreeSlot[] shouldBeLocked;

    private bool isHighlighted = false;
    [SerializeField] public bool unlocked = false; // UI上でのアンロック状態

    public bool IsUnlocked => unlocked;

    private void OnValidate()
    {
        // 旧来のフィールドを使用するので、skillName をそのまま表示用の名前にする
        gameObject.name = "SkillTreeSlot_UI - " + skillName;

        // highlightColor が未設定の場合、デフォルト値をセット
        if (highlightColor.Equals(default(Color)))
        {
            highlightColor = new Color(0, 1, 1, 130f / 255f);
        }
    }

    private void Awake()
    {
        uiManager = GetComponentInParent<UIManager>();
        skillImage = GetComponent<Image>();
    }

    private void Start()
    {
        skillImage.color = unlocked ? Color.white : lockedSkillColor;
    }

    private void Update()
    {
        bool isCurrentlySelected = EventSystem.current.currentSelectedGameObject == gameObject;

        if (isCurrentlySelected && !isHighlighted)
        {
            HighlightSlot();
            uiManager.skillToolTipInSkillTree.ShowToolTip(skillName, skillDescription, SkillCost);
        }
        else if (!isCurrentlySelected && isHighlighted)
        {
            UnhighlightSlot();
        }
    }

    private void HighlightSlot()
    {
        skillImage.color = highlightColor;
        isHighlighted = true;
    }

    private void OnDisable()
    {
        if (uiManager != null && uiManager.skillToolTipInSkillTree != null)
        {
            uiManager.skillToolTipInSkillTree.HideToolTip();
        }
    }

    public void OnSkillSlotSelected()
    {
        if (uiSkillTreeTabState != null)
        {
            uiSkillTreeTabState.ShowSkillPopup(this);
        }
    }

    /// <summary>
    /// ユーザーがこのスロットをアンロックする操作で呼ばれる
    /// </summary>
    public void UnlockSkillSlot()
    {
        unlocked = true;
        skillImage.color = Color.white;
        Debug.Log($"Skill '{skillName}' unlocked!");
    }

    private void UnhighlightSlot()
    {
        skillImage.color = unlocked ? Color.white : lockedSkillColor;
        isHighlighted = false;
    }

    public void RefreshUI()
    {
        if (isHighlighted)
            skillImage.color = highlightColor;
        else
            skillImage.color = unlocked ? Color.white : lockedSkillColor;
    }

    // ISaveManager の実装
    public void LoadData(GameData _data)
    {
        unlocked = _data.LoadSkillState(skillName);
        skillImage.color = unlocked ? Color.white : lockedSkillColor;
    }

    public void SaveData(ref GameData _data)
    {
        _data.SaveSkillState(skillName, unlocked);
    }
}
