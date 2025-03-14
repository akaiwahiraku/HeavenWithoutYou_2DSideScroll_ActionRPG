using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class UI_SkillSetSlot : MonoBehaviour, ISelectHandler, ISubmitHandler
{
    [SerializeField] private SkillCategory skillCategory;
    [SerializeField] private TextMeshProUGUI skillNameText;
    private UIManager uiManager;

    public SkillCategory SkillCategory { get { return skillCategory; } }

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (skillNameText == null)
        {
            skillNameText = GetComponentInChildren<TextMeshProUGUI>();
            if (skillNameText == null)
            {
                Debug.LogError("[UI_SkillSetSlot] skillNameText is not assigned on " + gameObject.name);
            }
        }
    }

    private void OnEnable()
    {
        // 直接 UpdateSlotDisplay() を呼ぶのではなく、遅延させる
        StartCoroutine(DelayedUpdateSlotDisplay());
    }

    private IEnumerator DelayedUpdateSlotDisplay()
    {
        yield return null; // 1フレーム待つ
        UpdateSlotDisplay();
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log($"[UI_SkillSetSlot] OnSelect => {gameObject.name} (category={skillCategory})");
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Debug.Log($"[UI_SkillSetSlot] OnSubmit => open skill selection for {skillCategory}");
        if (uiManager != null)
        {
            uiManager.OpenSkillSelectionPanel(skillCategory, this);
        }
        else
        {
            Debug.LogWarning("[UI_SkillSetSlot] UIManager reference is null on " + gameObject.name);
        }
    }

    public void UpdateSlotDisplay()
    {
        if (UIManager.instance == null || SkillManager.instance == null)
        {
            Debug.LogWarning("[UI_SkillSetSlot] UIManager or SkillManager instance is null");
            return;
        }

        Skill selected = SkillManager.instance.GetSelectedSkill(skillCategory);
        Debug.Log($"[UI_SkillSetSlot.UpdateSlotDisplay] category={skillCategory}, selected={(selected != null ? selected.skillName : "None")}");
        if (skillNameText != null)
        {
            skillNameText.text = (selected != null) ? selected.skillName : "None";
        }
    }
}

