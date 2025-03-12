using System.Collections;
using System.Collections.Generic;
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

    [Header("Stat Preview Panel")]
    [SerializeField] private GameObject statPreviewPanel;
    [SerializeField] private TextMeshProUGUI strengthPreviewText;
    [SerializeField] private TextMeshProUGUI agilityPreviewText;
    [SerializeField] private TextMeshProUGUI intelligencePreviewText;
    [SerializeField] private TextMeshProUGUI vitalityPreviewText;
    [SerializeField] private TextMeshProUGUI damagePreviewText;
    [SerializeField] private TextMeshProUGUI critChancePreviewText;
    [SerializeField] private TextMeshProUGUI critPowerPreviewText;
    [SerializeField] private TextMeshProUGUI overDrivePreviewText;
    [SerializeField] private TextMeshProUGUI maxHealthPreviewText;
    [SerializeField] private TextMeshProUGUI armorPreviewText;
    [SerializeField] private TextMeshProUGUI evasionPreviewText;
    [SerializeField] private TextMeshProUGUI magicResistancePreviewText;
    [SerializeField] private TextMeshProUGUI fireDamagePreviewText;
    [SerializeField] private TextMeshProUGUI iceDamagePreviewText;
    [SerializeField] private TextMeshProUGUI lightningDamagePreviewText;

    [Header("Skill Slot References")]
    public UI_SkillTreeSlot currentSkillSlot;
    public UI_SkillTreeSlot unlockedSkillSlot;

    #endregion

    public bool menuJustClosed = false;


    #region Private Fields

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

    #region Unity Lifecycle

    private void Awake()
    {
        // シングルトン初期化
        if (instance != null)
        {
            //Debug.LogWarning("[UIManager] Another instance already exists, destroying this one.");
            Destroy(gameObject);
            return;
        }
        instance = this;

        eventSystem = EventSystem.current;
        joystickInputManager = GetComponentInChildren<JoystickInputManager>();

        // 各パネルはInspectorから設定済みとする

        // 初期状態はメニューを閉じる
        CloseMenuUI();

        // 入力アクションの初期化
        openMenuAction = uiControls.FindAction("UI/OpenMenu");
        submitAction = uiControls.FindAction("UI/Submit");
        cancelAction = uiControls.FindAction("UI/Cancel");
        navigateAction = uiControls.FindAction("UI/Navigate");

        openMenuAction.performed += _ => OpenToggleMenu();
        // ここで、メニューが開いている場合のみ HandleCancel() を呼ぶように変更
        cancelAction.performed += ctx => {
            if (isMenuOpen)
            {
                HandleCancel();
            }
            // メニューが閉じている場合は何もしない（入力は他のシステムで処理される）
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

    #endregion

    #region Menu Management

    /// <summary>
    /// メニューのON/OFFを切り替えます。
    /// </summary>
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


    /// <summary>
    /// パネルの切り替えと状態遷移を行います。
    /// </summary>
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
        else
        {
            //Debug.LogWarning("Unknown panel passed to SwitchTo: " + panel.name);
        }

        lastActivePanel = panel;
        //Debug.Log("LastActivePanel updated to: " + lastActivePanel.name);
    }

    /// <summary>
    /// メニューを閉じ、ゲーム内UIを復帰させます。
    /// </summary>
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

    /// <summary>
    /// キャンセル入力時の処理（例：メニューを閉じる）
    /// </summary>
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

    #region Cursor & Souls UI

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

    #endregion

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
        //Debug.Log("[UIManager] ShowStatPreview called for: " + hoveredEquipment.itemName);
        if (hoveredEquipment == null)
            return;

        ItemData_Equipment currentEquipment = Inventory.instance.GetEquipment(hoveredEquipment.equipmentType);
        if (currentEquipment != null)
            Debug.Log("[UIManager] currentEquipment: " + currentEquipment.itemName);
        else
            Debug.Log("[UIManager] No current equipment for type: " + hoveredEquipment.equipmentType);

        int diffStrength = hoveredEquipment.strength - (currentEquipment != null ? currentEquipment.strength : 0);
        int diffAgility = hoveredEquipment.agility - (currentEquipment != null ? currentEquipment.agility : 0);
        int diffIntelligence = hoveredEquipment.intelligence - (currentEquipment != null ? currentEquipment.intelligence : 0);
        int diffVitality = hoveredEquipment.vitality - (currentEquipment != null ? currentEquipment.vitality : 0);

        int diffDamage = (hoveredEquipment.damage + hoveredEquipment.strength)
                         - (currentEquipment != null ? (currentEquipment.damage + currentEquipment.strength) : 0);
        int diffCritChance = (hoveredEquipment.critChance + hoveredEquipment.agility)
                             - (currentEquipment != null ? (currentEquipment.critChance + currentEquipment.agility) : 0);
        int diffCritPower = (hoveredEquipment.critPower + hoveredEquipment.strength)
                            - (currentEquipment != null ? (currentEquipment.critPower + currentEquipment.strength) : 0);

        int diffArmor = hoveredEquipment.armor - (currentEquipment != null ? currentEquipment.armor : 0);
        int diffEvasion = (hoveredEquipment.evasion + hoveredEquipment.agility)
                          - (currentEquipment != null ? (currentEquipment.evasion + currentEquipment.agility) : 0);
        int diffMagicRes = (hoveredEquipment.magicResistance + hoveredEquipment.intelligence * 3)
                           - (currentEquipment != null ? (currentEquipment.magicResistance + currentEquipment.intelligence * 3) : 0);

        int diffFireDamage = hoveredEquipment.fireDamage - (currentEquipment != null ? currentEquipment.fireDamage : 0);
        int diffIceDamage = hoveredEquipment.iceDamage - (currentEquipment != null ? currentEquipment.iceDamage : 0);
        int diffLightningDamage = hoveredEquipment.lightningDamage - (currentEquipment != null ? currentEquipment.lightningDamage : 0);

        strengthPreviewText.text = FormatDiff(diffStrength);
        agilityPreviewText.text = FormatDiff(diffAgility);
        intelligencePreviewText.text = FormatDiff(diffIntelligence);
        vitalityPreviewText.text = FormatDiff(diffVitality);

        damagePreviewText.text = FormatDiff(diffDamage);
        critChancePreviewText.text = FormatDiff(diffCritChance);
        critPowerPreviewText.text = FormatDiff(diffCritPower);
        //overDrivePreviewText.text = FormatDiff(diffOverDrive);

        //maxHealthPreviewText.text = FormatDiff(diffMaxHealth);
        armorPreviewText.text = FormatDiff(diffArmor);
        evasionPreviewText.text = FormatDiff(diffEvasion);
        magicResistancePreviewText.text = FormatDiff(diffMagicRes);

        fireDamagePreviewText.text = FormatDiff(diffFireDamage);
        iceDamagePreviewText.text = FormatDiff(diffIceDamage);
        lightningDamagePreviewText.text = FormatDiff(diffLightningDamage);

        statPreviewPanel.SetActive(true);
        //Debug.Log("[UIManager] statPreviewPanel active: " + statPreviewPanel.activeSelf);
    }

    private string FormatDiff(int diff)
    {
        if (diff == 0)
            return "";
        string color = diff > 0 ? "green" : "red";
        // 正の数の場合は+記号を付ける
        string sign = diff > 0 ? "+" : "";
        return $"<color={color}>{sign}{diff}</color>";
    }

    public void ClearStatPreview()
    {
        statPreviewPanel.SetActive(false);
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

    /// <summary>
    /// UI_SkillSetSlotから呼ばれる。スキル選択パネルを開き、現在のスロットを記録します。
    /// </summary>
    public void OpenSkillSelectionPanel(SkillCategory category, UI_SkillSetSlot slot)
    {
        currentSelectedSkillSlot = slot;
        if (skillSelectionPanel != null)
        {
            skillSelectionPanel.Open(category, slot);
        }
        else
        {
           // Debug.LogWarning("[UIManager] skillSelectionPanel is not assigned.");
        }
    }


    public void OpenEquipmentSelectionPanel(EquipmentType equipmentType, UI_EquipmentSetSlot slot)
    {
        if (equipmentSelectionPanel != null)
        {
            equipmentSelectionPanel.Open(equipmentType, slot);
        }
        else
        {
            //Debug.LogWarning("EquipmentSelectionPanel is not assigned in UIManager.");
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

    /// <summary>
    /// 指定のGameObjectが親の子階層にあるかどうかを返す
    /// </summary>
    private bool IsChildOf(GameObject child, GameObject parent)
    {
        return child.transform.IsChildOf(parent.transform);
    }

}
