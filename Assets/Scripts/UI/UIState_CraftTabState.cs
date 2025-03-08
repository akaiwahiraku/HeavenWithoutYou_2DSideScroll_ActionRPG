using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIState_CraftTabState : UIState
{
    private UIManager uiManager;
    private GameObject craftPanel;
    // クラフトタブのヘッダー用ボタンのパス（プロジェクトの階層構造に合わせて変更してください）
    private const string MENU_HEADER_CRAFT_PATH = "Menu's Header/Craft";

    // 4引数のコンストラクタ（craftHeaderButton はここでは使用せず、パスから取得する）
    public UIState_CraftTabState(UIStateMachine machine, UIManager uiManager, GameObject craftPanel, Button craftHeaderButton)
        : base(machine)
    {
        this.uiManager = uiManager;
        this.craftPanel = craftPanel;
    }

    public override void Enter()
    {
        // クラフトパネルを表示
        craftPanel.SetActive(true);
        Debug.Log("Enter CraftTabState: craftPanel active = " + craftPanel.activeSelf);

        // クラフトUIの初期設定（クラフトリスト、クラフトウィンドウの更新など）
        SetupCraftUI();

        // 次フレームにクラフトタブのヘッダーをデフォルト選択にするコルーチンを開始
        uiManager.StartCoroutine(SelectDefaultCraftButton());
    }

    private IEnumerator SelectDefaultCraftButton()
    {
        // UI描画が完了するまで待機
        yield return new WaitForEndOfFrame();
        Debug.Log("SelectDefaultCraftButton coroutine executed.");

        // まず、指定した固定パスからクラフトタブのヘッダー用ボタンを探す
        var craftHeaderTransform = craftPanel.transform.Find(MENU_HEADER_CRAFT_PATH);
        if (craftHeaderTransform != null)
        {
            var craftHeaderButton = craftHeaderTransform.GetComponent<Button>();
            if (craftHeaderButton != null && craftHeaderButton.gameObject.activeInHierarchy)
            {
                // 必要ならボタンをアクティブ化
                craftHeaderButton.gameObject.SetActive(true);
                // イベントシステムで選択
                EventSystem.current.SetSelectedGameObject(craftHeaderButton.gameObject);
                Debug.Log("Selected 'Craft' button from Menu's Header as default: " + craftHeaderButton.gameObject.name);
                yield break;
            }
            else
            {
                Debug.LogWarning("'Craft' Button found but not active or missing Button component.");
            }
        }
        else
        {
            Debug.LogWarning("Could not find transform path for craft header: " + MENU_HEADER_CRAFT_PATH);
        }

        // fallback: craftPanel内の最初のアクティブかつインタラクティブなボタンを選択
        var defaultButton = craftPanel.GetComponentInChildren<Button>(true);
        if (defaultButton != null && defaultButton.gameObject.activeInHierarchy && defaultButton.interactable)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
            Debug.Log("Fallback: Selected default button from craftPanel: " + defaultButton.gameObject.name);
        }
        else
        {
            Debug.LogWarning("No active interactable button found in craftPanel.");
        }
    }

    public override void Exit()
    {
        // クラフトパネルを非表示にする
        craftPanel.SetActive(false);
        Debug.Log("Exit CraftTabState");
    }

    public override void Update()
    {
        if (uiManager.submitAction != null && uiManager.submitAction.triggered)
        {
            HandleCraftSubmit();
        }
    }

    /// <summary>
    /// クラフトUIの初期設定（クラフトリスト、クラフトウィンドウの更新など）
    /// </summary>
    private void SetupCraftUI()
    {
        if (uiManager.craftWindow != null)
        {
            // 必要に応じてクラフトウィンドウの初期化処理を実施
            // uiManager.craftWindow.SetupCraftWindow(...);
        }

        // クラフトリストの更新
        var craftList = craftPanel.GetComponentInChildren<UI_CraftList>();
        if (craftList != null)
        {
            craftList.SetupCraftList();
            craftList.SetupDefaultCraftWindow();
        }
    }

    /// <summary>
    /// SubmitAction 時に、現在選択中のクラフトスロットを処理する
    /// </summary>
    private void HandleCraftSubmit()
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject;
        if (currentSelected == null)
        {
            Debug.Log("HandleCraftSubmit: No object selected.");
            return;
        }

        var craftSlot = currentSelected.GetComponent<UI_CraftSlot>();
        if (craftSlot != null)
        {
            craftSlot.OnSelect(null);
            Debug.Log("HandleCraftSubmit: Processed craft slot " + currentSelected.name);
        }
    }
}
