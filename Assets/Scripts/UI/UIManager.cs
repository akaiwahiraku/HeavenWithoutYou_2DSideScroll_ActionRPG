using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ISaveManager
{
    #region Inspector Settings

    [Header("Input Action Assets")]
    [SerializeField] private InputActionAsset uiControls;
    [SerializeField] private InputActionAsset joystickControls;

    [Header("Cursor Settings")]
    [SerializeField] private RectTransform pointerCursor;
    [SerializeField] private Vector2 cursorOffset = new Vector2(20, 20);

    [Header("Skill Unlock Popup")]
    [SerializeField] public GameObject skillUnlockPopup;
    [SerializeField] public Button yesButton;
    [SerializeField] public Button noButton;

    [Header("Souls Info")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increaseRate = 100;

    [Space]
    [Header("UI References")]
    [SerializeField] private GameObject menuUI;       // メインメニューCanvas（親）
    [SerializeField] private GameObject inGameUI;     // ゲーム中のUI
    [SerializeField] public UI_SkillSelectionPanel skillSelectionPanel; // Inspectorで割り当て
    [SerializeField] public UI_EquipmentSelectionPanel equipmentSelectionPanel;
    [SerializeField] private UI_StatPreview statPreview;

    // Menu内の各パネル（子オブジェクト）
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;

    // 各タブボタン（Inspectorで設定）
    [SerializeField] public Button characterTabButton;
    [SerializeField] public Button skillTreeTabButton;
    [SerializeField] public Button craftTabButton;
    [SerializeField] public Button optionsTabButton;

    [Header("ToolTips and Craft Window")]
    public UI_ToolTip_Skill_InSkillTree skillToolTipInSkillTree;
    public UI_ToolTip_Skill_InCharacter skillToolTipInCharacter;
    public UI_ToolTip_Item itemToolTip;
    public UI_CraftWindow craftWindow;

    [Header("Volume Settings")]
    [SerializeField] private UI_VolumeSlider[] volumeSettings;

    [Header("UI State Machine")]
    [SerializeField] private UIStateMachine uiStateMachine;

    [Header("Skill Slot References")]
    public UI_SkillTreeSlot currentSkillSlot;
    public UI_SkillTreeSlot unlockedSkillSlot;

    public bool menuJustClosed = false;

    public static UIManager instance;

    // 入力アクション関連
    private JoystickInputManager joystickInputManager;
    private EventSystem eventSystem;
    private InputAction openMenuAction;
    public InputAction submitAction;
    private InputAction cancelAction;
    private InputAction navigateAction;

    // メニュー／ポップアップの状態フラグ
    public bool isMenuOpen = false;
    public bool isPopupActive = false;
    public bool isSkillTreeActive = false;
    public bool isCharacterActive = false;
    public bool canSubmit = true;

    // 最後にアクティブだったパネル
    private GameObject lastActivePanel;

    // スキル選択時に現在操作中のスロット
    private UI_SkillSetSlot currentSelectedSkillSlot;

    #endregion


    private void Awake()
    {
        // シングルトン初期化
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        eventSystem = EventSystem.current;
        joystickInputManager = GetComponentInChildren<JoystickInputManager>();

        // 初期状態はメニューを閉じる
        CloseMenuUI();

        // 入力アクションの初期化
        openMenuAction = uiControls.FindAction("UI/OpenMenu");
        submitAction = uiControls.FindAction("UI/Submit");
        cancelAction = uiControls.FindAction("UI/Cancel");
        navigateAction = uiControls.FindAction("UI/Navigate");

        openMenuAction.performed += _ => OpenToggleMenu();
        // ここで、メニューが開いている場合のみ HandleCancel() を呼ぶように変更
        cancelAction.performed += ctx =>
        {
            if (isMenuOpen)
            {
                HandleCancel();
            }
        };
    }

    private void Start()
    {
        // メニューを閉じた状態で開始
        CloseMenuUI();

        // ツールチップの初期設定
        if (itemToolTip != null)
        {
            itemToolTip.gameObject.SetActive(true);
            itemToolTip.HideToolTip();
        }
        if (skillToolTipInSkillTree != null)
        {
            skillToolTipInSkillTree.HideToolTip();
        }
        if (skillToolTipInCharacter != null)
        {
            skillToolTipInCharacter.HideToolTip();
        }

        if (skillUnlockPopup != null)
            skillUnlockPopup.SetActive(false);

        if (pointerCursor != null)
            pointerCursor.gameObject.SetActive(false);

        // 初期状態：キャラクターパネルを表示
        characterUI.SetActive(true);
        if (skillTreeUI != null) skillTreeUI.SetActive(false);
        if (craftUI != null) craftUI.SetActive(false);
        if (optionsUI != null) optionsUI.SetActive(false);
        lastActivePanel = characterUI;
    }

    private void Update()
    {

        UpdatePointerCursor();
        UpdateSoulsUI();

        // Submit入力の一括管理（連続入力防止）
        if (submitAction.triggered && canSubmit)
        {
            StartCoroutine(DisableSubmitTemporarily());
        }

        // 選択状態によってツールチップの表示／非表示を更新する
        UpdateToolTipVisibility();
    }

    private void OnEnable()
    {
        uiControls.Enable();
        joystickControls.Enable();
    }

    private void OnDisable()
    {
        uiControls.Disable();
        joystickControls.Disable();
    }


    #region Menu Management

    /// メニューのON/OFFを切り替えます。
    private void OpenToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (isMenuOpen)
        {
            // メニュー表示時の処理...
            menuUI.SetActive(true);
            inGameUI.SetActive(false);
            SwitchTo(characterUI);
            if (GameManager.instance != null)
                GameManager.instance.PauseGame(true);
            // ジャンプアクションを無効化
            var jumpAction = joystickControls.FindAction("Player/Jump");
            if (jumpAction != null)
                jumpAction.Disable();
        }
        else
        {
            // メニューが閉じられるとき
            CloseMenuUI();
            inGameUI.SetActive(true);
            if (GameManager.instance != null)
                GameManager.instance.PauseGame(false);
            // メニュー閉鎖直後のフラグをセット
            menuJustClosed = true;
            StartCoroutine(ResetMenuJustClosedFlag());
        }
    }

    /// パネルの切り替えと状態遷移を行います。
    public void SwitchTo(GameObject panel)
    {
        //Debug.Log("SwitchTo() called. Target panel: " + panel.name);

        // 全パネルを非表示に
        if (characterUI != null) characterUI.SetActive(false);
        if (skillTreeUI != null) skillTreeUI.SetActive(false);
        if (craftUI != null) craftUI.SetActive(false);
        if (optionsUI != null) optionsUI.SetActive(false);

        // 選択状態をクリア
        eventSystem.SetSelectedGameObject(null);

        // 状態遷移（UIStateの切り替え）
        if (panel == characterUI)
        {
            uiStateMachine.ChangeState(new UIState_CharacterTabState(uiStateMachine, this, characterUI));
        }
        else if (panel == skillTreeUI)
        {
            uiStateMachine.ChangeState(new UIState_SkillTreeTabState(uiStateMachine, this, skillTreeUI, skillTreeTabButton));
        }
        else if (panel == craftUI)
        {
            uiStateMachine.ChangeState(new UIState_CraftTabState(uiStateMachine, this, craftUI, craftTabButton));
        }
        else if (panel == optionsUI)
        {
            uiStateMachine.ChangeState(new UIState_OptionTabState(uiStateMachine, this, optionsUI, optionsTabButton));
        }
 
        lastActivePanel = panel;
    }

    /// メニューを閉じ、ゲーム内UIを復帰させます。
    private void CloseMenuUI()
    {
        menuUI.SetActive(false);
        inGameUI.SetActive(true);

        if (GameManager.instance != null)
        {
            GameManager.instance.PauseGame(false);
        }

        eventSystem.SetSelectedGameObject(null);
        joystickInputManager?.EnableJoystickControl(true);

        // メニューが閉じたら、ジャンプアクションを有効化する
        var jumpAction = joystickControls.FindAction("Player/Jump");
        if (jumpAction != null)
        {
            jumpAction.Enable();
        }
    }

    /// キャンセル入力時の処理（例：メニューを閉じる）
    private void HandleCancel()
    {
        if (isMenuOpen)
        {
            OpenToggleMenu();
            // メニューが閉じた直後はフラグを立てる
            menuJustClosed = true;
            StartCoroutine(ResetMenuJustClosedFlag());
        }
    }

    #endregion


    private void UpdatePointerCursor()
    {
        GameObject selectedObj = eventSystem.currentSelectedGameObject;

        if (selectedObj != null && (selectedObj.GetComponent<Button>() != null ||
            selectedObj.GetComponent<Slider>() != null || selectedObj.GetComponent<Selectable>() != null))
        {
            if (pointerCursor != null)
            {
                pointerCursor.gameObject.SetActive(true);
                RectTransform rt = selectedObj.GetComponent<RectTransform>();
                if (rt != null)
                {
                    if (selectedObj.GetComponent<Slider>() != null)
                    {
                        Vector3 leftEdgeWorld = rt.TransformPoint(new Vector3(rt.rect.xMin, 0, 0));
                        pointerCursor.position = leftEdgeWorld;
                    }
                    else
                    {
                        pointerCursor.position = rt.position + (Vector3)cursorOffset;
                    }
                }
            }
        }
        else
        {
            if (pointerCursor != null)
                pointerCursor.gameObject.SetActive(false);
        }
    }

    private void UpdateSoulsUI()
    {
        if (CurrencyManager.instance != null)
        {
            float currency = CurrencyManager.instance.GetCurrency();
            if (soulsAmount < currency)
                soulsAmount += Time.deltaTime * increaseRate;
            else
                soulsAmount = currency;

            if (currentSouls != null)
                currentSouls.text = ((int)soulsAmount).ToString();
        }
    }


    #region Save/Load Data

    public void LoadData(GameData _data)
    {
        foreach (var pair in _data.volumeSettings)
        {
            foreach (UI_VolumeSlider item in volumeSettings)
            {
                if (item.parameter == pair.Key)
                    item.LoadSlider(pair.Value);
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();
        foreach (UI_VolumeSlider item in volumeSettings)
        {
            _data.volumeSettings.Add(item.parameter, item.slider.value);
        }
    }

    #endregion


    #region Stat Preview

    public void ShowStatPreview(ItemData_Equipment hoveredEquipment)
    {
        if (statPreview != null)
        {
            statPreview.ShowStatPreview(hoveredEquipment);
        }
    }

    public void ClearStatPreview()
    {
        if (statPreview != null)
        {
            statPreview.ClearStatPreview();
        }
    }

    #endregion


    private IEnumerator ResetMenuJustClosedFlag()
    {
        yield return new WaitForSeconds(0.1f);
        menuJustClosed = false;
    }



    private IEnumerator DisableSubmitTemporarily()
    {
        canSubmit = false;
        yield return new WaitForSeconds(0.1f);
        canSubmit = true;
    }

    /// UI_SkillSetSlotから呼ばれる。スキル選択パネルを開き、現在のスロットを記録します。
    public void OpenSkillSelectionPanel(SkillCategory category, UI_SkillSetSlot slot)
    {
        currentSelectedSkillSlot = slot;
        if (skillSelectionPanel != null)
        {
            skillSelectionPanel.Open(category, slot);
        }
    }


    public void OpenEquipmentSelectionPanel(EquipmentType equipmentType, UI_EquipmentSetSlot slot)
    {
        if (equipmentSelectionPanel != null)
        {
            equipmentSelectionPanel.Open(equipmentType, slot);
        }
    }

    // UIManager.cs の Update() の末尾などに呼び出す
    private void UpdateToolTipVisibility()
    {
        GameObject selectedObj = eventSystem.currentSelectedGameObject;

        // EquipmentSelectionPanelが開いている場合は、その子オブジェクトでないならツールチップを隠す
        if (equipmentSelectionPanel != null && equipmentSelectionPanel.gameObject.activeSelf)
        {
            if (selectedObj == null || !IsChildOf(selectedObj, equipmentSelectionPanel.gameObject))
            {
                itemToolTip.HideToolTip();
            }
        }
        else
        {
            // パネルが開いていない場合は常に非表示
            itemToolTip.HideToolTip();
        }

        // 同様に、SkillSelectionPanelが開いている場合は、その子でないならSkillツールチップも非表示に
        if (skillSelectionPanel != null && skillSelectionPanel.gameObject.activeSelf)
        {
            if (selectedObj == null || !IsChildOf(selectedObj, skillSelectionPanel.gameObject))
            {
                skillToolTipInCharacter.HideToolTip();
            }
        }
        else
        {
            skillToolTipInCharacter.HideToolTip();
        }
    }

    /// 指定のGameObjectが親の子階層にあるかどうかを返す
    private bool IsChildOf(GameObject child, GameObject parent)
    {
        return child.transform.IsChildOf(parent.transform);
    }

}