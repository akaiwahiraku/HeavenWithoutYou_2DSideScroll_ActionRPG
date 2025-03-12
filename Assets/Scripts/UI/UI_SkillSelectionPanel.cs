using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UI_SkillSelectionPanel : MonoBehaviour
{
    [SerializeField] public Transform skillListContainer; // 候補スキルボタンの配置親
    [SerializeField] private GameObject skillButtonPrefab; // スキルボタンPrefab

    private SkillCategory currentCategory;
    private UI_SkillSetSlot currentSlot;
    private UIManager uiManager;

    // 初期選択対象：候補スキルがあればその先頭、なければ currentSlot
    public GameObject firstOption;

    // 候補スキルボタンを保持するリスト（有効な GameObject 参照のみ）
    private List<GameObject> candidateButtons = new List<GameObject>();

    // パネル閉鎖処理中フラグ
    private bool isClosing = false;
    private float lastForceTime = 0f;

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        gameObject.SetActive(false);
        //Debug.Log("[UI_SkillSelectionPanel] Awake() called.");
    }

    private void Update()
    {
        if (isClosing) return;
        if (!gameObject.activeInHierarchy) return;

        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
        //Debug.Log($"[UI_SkillSelectionPanel] Update: currentSelected = {(currentSelected != null ? currentSelected.name : "null")}");

        // 強制選択チェックは一定時間間隔ごとに実施
        if (Time.unscaledTime - lastForceTime < 0.2f) return;
        lastForceTime = Time.unscaledTime;

        ValidateCurrentSelection(currentSelected);
    }
    /// <summary>
    /// SkillSelectedPanelが開いているときに、パネル外のボタンを選択しようとすると強制的にカーソルをfirstOptionに戻す
    /// </summary>
    private void ValidateCurrentSelection(GameObject currentSelected)
    {
        // パネル閉鎖中は検証処理をスキップする
        if (isClosing) return;

        if (candidateButtons.Count > 0)
        {
            bool isInCandidate = false;
            if (currentSelected != null)
            {
                foreach (GameObject candidate in candidateButtons)
                {
                    // currentSelectedが候補ボタンまたはその子オブジェクトならOK
                    if (currentSelected == candidate || currentSelected.transform.IsChildOf(candidate.transform))
                    {
                        isInCandidate = true;
                        break;
                    }
                }
            }
            if (!isInCandidate)
            {
                if (firstOption != null && firstOption.activeInHierarchy)
                {
                    //Debug.LogWarning($"[UI_SkillSelectionPanel] Update: Current selection is invalid or outside panel; forcing selection to firstOption: {firstOption.name}");
                    Button btn = firstOption.GetComponent<Button>();
                    if (btn != null)
                        btn.Select();
                    else
                        EventSystem.current.SetSelectedGameObject(firstOption);
                }
                else
                {
                    //Debug.LogWarning("[UI_SkillSelectionPanel] Update: firstOption is not valid for selection.");
                }
            }
        }
        else
        {
            // 候補が空の場合、選択がnullならcurrentSlotに戻す
            if (currentSelected == null && currentSlot != null && currentSlot.gameObject.activeInHierarchy)
            {
                //Debug.LogWarning($"[UI_SkillSelectionPanel] Update: No selection; forcing selection to currentSlot: {currentSlot.gameObject.name}");
                Button btn = currentSlot.GetComponent<Button>();
                if (btn != null)
                    btn.Select();
                else
                    EventSystem.current.SetSelectedGameObject(currentSlot.gameObject);
            }
        }
    }


    /// <summary>
    /// パネルを開く。候補スキルがあればボタン群を生成し、なければ直ちに currentSlot に戻る。
    /// </summary>
    public void Open(SkillCategory category, UI_SkillSetSlot slot)
    {
        currentCategory = category;
        currentSlot = slot;
        //Debug.Log($"[UI_SkillSelectionPanel] Open() called. Category: {category}, currentSlot: {(currentSlot != null ? currentSlot.gameObject.name : "null")}");
        EventSystem.current.SetSelectedGameObject(null);
        isClosing = false;
        gameObject.SetActive(true);

        PopulateSkillList();

        // 候補スキルがある場合のみ初期選択設定を行う
        if (firstOption != null)
        {
            //Debug.Log($"[UI_SkillSelectionPanel] firstOption is set to: {firstOption.name}. Starting SetInitialSelection().");
            StartCoroutine(SetInitialSelection());
        }
    }

    /// <summary>
    /// 候補スキルボタンを生成する。
    /// 候補スキルがあればそのボタン群を生成し、なければ firstOption を currentSlot に設定し直ちに閉じる。
    /// </summary>
    private void PopulateSkillList()
    {
        // 候補ボタンリストをクリア
        candidateButtons.Clear();
        foreach (Transform child in skillListContainer)
        {
            Destroy(child.gameObject);
        }
        //Debug.Log("[UI_SkillSelectionPanel] Cleared existing children in skillListContainer.");

        List<Skill> unlocked = SkillManager.instance.GetUnlockedSkills(currentCategory);
        //Debug.Log($"[UI_SkillSelectionPanel] Number of unlocked skills for {currentCategory}: {unlocked.Count}");

        if (unlocked.Count > 0)
        {
            foreach (Skill skill in unlocked)
            {
                GameObject btnObj = Instantiate(skillButtonPrefab, skillListContainer);
                TextMeshProUGUI label = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                Button btn = btnObj.GetComponent<Button>();

                if (label != null)
                    label.text = skill.skillName;
                //else
                    //Debug.LogWarning("[UI_SkillSelectionPanel] Missing TextMeshProUGUI on skill button prefab.");

                candidateButtons.Add(btnObj);
                btn.onClick.AddListener(() => OnSkillSelected(skill));

                // ツールチップ表示用 EventTrigger の設定
                EventTrigger trigger = btnObj.GetComponent<EventTrigger>();
                if (trigger == null)
                    trigger = btnObj.AddComponent<EventTrigger>();

                EventTrigger.Entry selectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Select };
                selectEntry.callback.AddListener((data) =>
                {
                    if (UIManager.instance != null && skill != null)
                    {
                        //Debug.Log($"[UI_SkillSelectionPanel] (OnSelect) Showing tool tip for skill '{skill.skillName}'");
                        UIManager.instance.skillToolTipInCharacter.ShowToolTip(skill.skillName, skill.description);
                    }
                });
                trigger.triggers.Add(selectEntry);

                EventTrigger.Entry deselectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Deselect };
                deselectEntry.callback.AddListener((data) =>
                {
                    if (UIManager.instance != null)
                    {
                        //Debug.Log("[UI_SkillSelectionPanel] (OnDeselect) Hiding tool tip");
                        UIManager.instance.skillToolTipInCharacter.HideToolTip();
                    }
                });
                trigger.triggers.Add(deselectEntry);
            }
            firstOption = candidateButtons[0];
            //Debug.Log($"[UI_SkillSelectionPanel] PopulateSkillList: firstOption set to candidate button: {firstOption.name}");
        }
        else
        {
            // 候補スキルがない場合は、firstOption を currentSlot に設定し、直ちに閉じる
            if (currentSlot != null)
            {
                firstOption = currentSlot.gameObject;
                //Debug.Log($"[UI_SkillSelectionPanel] No candidate skills: firstOption set to currentSlot: {firstOption.name}");
                StartCoroutine(CloseAndReturnFocus());
            }
            else
            {
                //Debug.LogWarning("[UI_SkillSelectionPanel] No candidate skills and currentSlot is null.");
            }
        }
    }

    /// <summary>
    /// 候補スキルボタンがクリックされたときの処理
    /// </summary>
    private void OnSkillSelected(Skill skill)
    {
        //Debug.Log($"[UI_SkillSelectionPanel] Skill selected: {skill.skillName}");
        SkillManager.instance.SetSelectedSkill(currentCategory, skill);

        if (currentSlot != null)
        {
            currentSlot.UpdateSlotDisplay();
        }
        else
        {
            //Debug.LogWarning("[UI_SkillSelectionPanel] currentSlot is null.");
        }

        Close();
    }

    /// <summary>
    /// パネルを閉じ、currentSlot にフォーカスを戻す
    /// </summary>
    public void Close()
    {
        //Debug.Log("[UI_SkillSelectionPanel] Close() called.");
        if (gameObject.activeInHierarchy && !isClosing)
        {
            isClosing = true;
            StartCoroutine(CloseAndReturnFocus());
        }
        else
        {
            //Debug.LogWarning("[UI_SkillSelectionPanel] Close() called when panel is inactive or already closing.");
        }
    }

    private IEnumerator CloseAndReturnFocus()
    {
        // 一旦選択状態をクリア
        EventSystem.current.SetSelectedGameObject(null);

        // UI更新が完全に反映されるまで待つ（ここでは EndOfFrame を利用）
        yield return new WaitForEndOfFrame();

        // または currentSlot がアクティブな状態になるまで待つ（必要に応じて）
        yield return new WaitUntil(() => currentSlot != null && currentSlot.gameObject.activeInHierarchy);

        // currentSlot が選択可能な状態か確認
        Button btn = currentSlot.GetComponent<Button>();
        if (btn != null)
        {
            //Debug.Log($"[UI_SkillSelectionPanel] Returning focus to {currentSlot.gameObject.name} using Button.Select().");
            btn.Select();
        }
        else
        {
            //Debug.Log($"[UI_SkillSelectionPanel] No Button component on {currentSlot.gameObject.name}. Using SetSelectedGameObject().");
            EventSystem.current.SetSelectedGameObject(currentSlot.gameObject);
        }

        // 必要に応じて、UI更新の余裕を持たせるため、さらに少し待つ
        yield return new WaitForSeconds(0.1f);

        // 候補ボタンのリストをクリアし、パネルを完全に非表示にする
        candidateButtons.Clear();
        gameObject.SetActive(false);
        isClosing = false;

        //Debug.Log("[UI_SkillSelectionPanel] CloseAndReturnFocus: Panel is now inactive.");
    }


    private IEnumerator SetInitialSelection()
    {
        yield return new WaitForSeconds(0.1f); // EventSystem の状態安定のため少し待つ
        if (firstOption != null && firstOption.activeInHierarchy)
        {
            //Debug.Log($"[UI_SkillSelectionPanel] SetInitialSelection: firstOption is valid: {firstOption.name}");
            EventSystem.current.SetSelectedGameObject(firstOption);
        }
        else if (currentSlot != null && currentSlot.gameObject.activeInHierarchy)
        {
            //Debug.LogWarning($"[UI_SkillSelectionPanel] SetInitialSelection: firstOption invalid; using currentSlot: {currentSlot.gameObject.name}");
            EventSystem.current.SetSelectedGameObject(currentSlot.gameObject);
        }
        else
        {
            //Debug.LogWarning("[UI_SkillSelectionPanel] SetInitialSelection: No valid initial selection target.");
        }
    }

    // ヘルパー：指定オブジェクトが、指定の親の子孫であるかどうかを返す
    private bool IsDescendantOf(GameObject obj, Transform parent)
    {
        return obj.transform.IsChildOf(parent);
    }
}
