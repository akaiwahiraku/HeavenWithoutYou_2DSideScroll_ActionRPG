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
    [SerializeField] private GameObject menuUI;       // ���C�����j���[Canvas�i�e�j
    [SerializeField] private GameObject inGameUI;     // �Q�[������UI
    [SerializeField] public UI_SkillSelectionPanel skillSelectionPanel; // Inspector�Ŋ��蓖��
    [SerializeField] public UI_EquipmentSelectionPanel equipmentSelectionPanel;


    // Menu���̊e�p�l���i�q�I�u�W�F�N�g�j
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;

    // �e�^�u�{�^���iInspector�Őݒ�j
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

    // ���̓A�N�V�����֘A
    private JoystickInputManager joystickInputManager;
    private EventSystem eventSystem;
    private InputAction openMenuAction;
    public InputAction submitAction;
    private InputAction cancelAction;
    private InputAction navigateAction;

    // ���j���[�^�|�b�v�A�b�v�̏�ԃt���O
    public bool isMenuOpen = false;
    public bool isPopupActive = false;
    public bool isSkillTreeActive = false;
    public bool isCharacterActive = false;
    public bool canSubmit = true;

    // �Ō�ɃA�N�e�B�u�������p�l��
    private GameObject lastActivePanel;

    // �X�L���I�����Ɍ��ݑ��쒆�̃X���b�g
    private UI_SkillSetSlot currentSelectedSkillSlot;



    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // �V���O���g��������
        if (instance != null)
        {
            Debug.LogWarning("[UIManager] Another instance already exists, destroying this one.");
            Destroy(gameObject);
            return;
        }
        instance = this;

        eventSystem = EventSystem.current;
        joystickInputManager = GetComponentInChildren<JoystickInputManager>();

        // �e�p�l����Inspector����ݒ�ς݂Ƃ���

        // ������Ԃ̓��j���[�����
        CloseMenuUI();

        // ���̓A�N�V�����̏�����
        openMenuAction = uiControls.FindAction("UI/OpenMenu");
        submitAction = uiControls.FindAction("UI/Submit");
        cancelAction = uiControls.FindAction("UI/Cancel");
        navigateAction = uiControls.FindAction("UI/Navigate");

        openMenuAction.performed += _ => OpenToggleMenu();
        // �����ŁA���j���[���J���Ă���ꍇ�̂� HandleCancel() ���ĂԂ悤�ɕύX
        cancelAction.performed += ctx => {
            if (isMenuOpen)
            {
                HandleCancel();
            }
            // ���j���[�����Ă���ꍇ�͉������Ȃ��i���͂͑��̃V�X�e���ŏ��������j
        };
    }

    private void Start()
    {
        // ���j���[�������ԂŊJ�n
        CloseMenuUI();

        // �c�[���`�b�v�̏����ݒ�
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

        // ������ԁF�L�����N�^�[�p�l����\��
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

        // Submit���͂̈ꊇ�Ǘ��i�A�����͖h�~�j
        if (submitAction.triggered && canSubmit)
        {
            StartCoroutine(DisableSubmitTemporarily());
        }

        // �I����Ԃɂ���ăc�[���`�b�v�̕\���^��\�����X�V����
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
    /// ���j���[��ON/OFF��؂�ւ��܂��B
    /// </summary>
    private void OpenToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (isMenuOpen)
        {
            // ���j���[�\�����̏���...
            menuUI.SetActive(true);
            inGameUI.SetActive(false);
            SwitchTo(characterUI);
            if (GameManager.instance != null)
                GameManager.instance.PauseGame(true);
            // �W�����v�A�N�V�����𖳌���
            var jumpAction = joystickControls.FindAction("Player/Jump");
            if (jumpAction != null)
                jumpAction.Disable();
        }
        else
        {
            // ���j���[��������Ƃ�
            CloseMenuUI();
            inGameUI.SetActive(true);
            if (GameManager.instance != null)
                GameManager.instance.PauseGame(false);
            // ���j���[������̃t���O���Z�b�g
            menuJustClosed = true;
            StartCoroutine(ResetMenuJustClosedFlag());
        }
    }


    /// <summary>
    /// �p�l���̐؂�ւ��Ə�ԑJ�ڂ��s���܂��B
    /// </summary>
    public void SwitchTo(GameObject panel)
    {
        Debug.Log("SwitchTo() called. Target panel: " + panel.name);

        // �S�p�l�����\����
        if (characterUI != null) characterUI.SetActive(false);
        if (skillTreeUI != null) skillTreeUI.SetActive(false);
        if (craftUI != null) craftUI.SetActive(false);
        if (optionsUI != null) optionsUI.SetActive(false);

        // �I����Ԃ��N���A
        eventSystem.SetSelectedGameObject(null);

        // ��ԑJ�ځiUIState�̐؂�ւ��j
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
            Debug.LogWarning("Unknown panel passed to SwitchTo: " + panel.name);
        }

        lastActivePanel = panel;
        Debug.Log("LastActivePanel updated to: " + lastActivePanel.name);
    }

    /// <summary>
    /// ���j���[����A�Q�[����UI�𕜋A�����܂��B
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

        // ���j���[��������A�W�����v�A�N�V������L��������
        var jumpAction = joystickControls.FindAction("Player/Jump");
        if (jumpAction != null)
        {
            jumpAction.Enable();
        }
    }

    /// <summary>
    /// �L�����Z�����͎��̏����i��F���j���[�����j
    /// </summary>
    private void HandleCancel()
    {
        if (isMenuOpen)
        {
            OpenToggleMenu();
            // ���j���[����������̓t���O�𗧂Ă�
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
        Debug.Log("[UIManager] ShowStatPreview called for: " + hoveredEquipment.itemName);
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
        Debug.Log("[UIManager] statPreviewPanel active: " + statPreviewPanel.activeSelf);
    }

    private string FormatDiff(int diff)
    {
        if (diff == 0)
            return "";
        string color = diff > 0 ? "green" : "red";
        // ���̐��̏ꍇ��+�L����t����
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
    /// UI_SkillSetSlot����Ă΂��B�X�L���I���p�l�����J���A���݂̃X���b�g���L�^���܂��B
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
            Debug.LogWarning("[UIManager] skillSelectionPanel is not assigned.");
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
            Debug.LogWarning("EquipmentSelectionPanel is not assigned in UIManager.");
        }
    }

    // UIManager.cs �� Update() �̖����ȂǂɌĂяo��
    private void UpdateToolTipVisibility()
    {
        GameObject selectedObj = eventSystem.currentSelectedGameObject;

        // EquipmentSelectionPanel���J���Ă���ꍇ�́A���̎q�I�u�W�F�N�g�łȂ��Ȃ�c�[���`�b�v���B��
        if (equipmentSelectionPanel != null && equipmentSelectionPanel.gameObject.activeSelf)
        {
            if (selectedObj == null || !IsChildOf(selectedObj, equipmentSelectionPanel.gameObject))
            {
                itemToolTip.HideToolTip();
            }
        }
        else
        {
            // �p�l�����J���Ă��Ȃ��ꍇ�͏�ɔ�\��
            itemToolTip.HideToolTip();
        }

        // ���l�ɁASkillSelectionPanel���J���Ă���ꍇ�́A���̎q�łȂ��Ȃ�Skill�c�[���`�b�v����\����
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
    /// �w���GameObject���e�̎q�K�w�ɂ��邩�ǂ�����Ԃ�
    /// </summary>
    private bool IsChildOf(GameObject child, GameObject parent)
    {
        return child.transform.IsChildOf(parent.transform);
    }

}
