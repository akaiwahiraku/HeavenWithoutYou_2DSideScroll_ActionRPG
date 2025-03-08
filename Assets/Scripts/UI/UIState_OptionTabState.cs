using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIState_OptionTabState : UIState
{
    private UIManager uiManager;
    private GameObject optionsPanel;
    // 固定パス：プロジェクトの階層構造に合わせて変更してください
    private const string MENU_HEADER_OPTION_PATH = "Menu's Header/Options";

    // 4引数のコンストラクタ（optionsHeaderButton は固定パスで取得するため使わない）
    public UIState_OptionTabState(UIStateMachine machine, UIManager uiManager, GameObject optionsPanel, Button optionsHeaderButton)
        : base(machine)
    {
        this.uiManager = uiManager;
        this.optionsPanel = optionsPanel;
    }

    public override void Enter()
    {
        // オプションパネルを表示
        optionsPanel.SetActive(true);
        // 必要であれば uiManager のフラグを更新（例：uiManager.isOptionsActive = true;）
        Debug.Log("Enter OptionTabState: optionsPanel active = " + optionsPanel.activeSelf);

        // オプションUIの初期設定
        SetupOptionsUI();

        // 次フレームに固定パスからオプションヘッダー用ボタンを選択する
        uiManager.StartCoroutine(SelectDefaultButton());
    }

    private IEnumerator SelectDefaultButton()
    {
        // UI描画が完全に終わるまで待機
        yield return new WaitForEndOfFrame();
        Debug.Log("SelectDefaultOptionButton coroutine executed for OptionTabState.");

        // 固定パスからオプションヘッダー用ボタンを取得
        var optionHeaderTransform = optionsPanel.transform.Find(MENU_HEADER_OPTION_PATH);
        if (optionHeaderTransform != null)
        {
            var optionHeaderButton = optionHeaderTransform.GetComponent<Button>();
            if (optionHeaderButton != null && optionHeaderButton.gameObject.activeInHierarchy)
            {
                // 必要であればボタンをアクティブ化
                optionHeaderButton.gameObject.SetActive(true);
                // イベントシステムで選択
                EventSystem.current.SetSelectedGameObject(optionHeaderButton.gameObject);
                Debug.Log("Selected 'Options' button from Menu's Header as default: " + optionHeaderButton.gameObject.name);
                yield break;
            }
            else
            {
                Debug.LogWarning("'Options' button found but it is not active or missing Button component.");
            }
        }
        else
        {
            Debug.LogWarning("Could not find transform path for options header: " + MENU_HEADER_OPTION_PATH);
        }

        // フォールバック：optionsPanel内の最初の有効かつインタラクティブなボタンを選択
        var defaultButton = optionsPanel.GetComponentInChildren<Button>(true);
        if (defaultButton != null && defaultButton.gameObject.activeInHierarchy && defaultButton.interactable)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
            Debug.Log("Fallback: Selected default button from optionsPanel: " + defaultButton.gameObject.name);
        }
        else
        {
            Debug.LogWarning("No active interactable button found in optionsPanel.");
        }
    }
    public override void Exit()
    {
        // オプションパネルを非表示
        optionsPanel.SetActive(false);
        Debug.Log("Exit OptionTabState");
    }

    public override void Update()
    {
        // Submitボタンが押されたときの処理
        if (uiManager.submitAction != null && uiManager.submitAction.triggered)
        {
            HandleOptionSubmit();
        }
    }


    /// <summary>
    /// オプションUIの初期設定（必要に応じた初期化処理）
    /// </summary>
    private void SetupOptionsUI()
    {
        // 例: UIManager の volumeSettings を使って各スライダーのリスナーを再設定するなど
        Debug.Log("SetupOptionsUI called in OptionTabState.");
    }

    /// <summary>
    /// Submitボタンが押されたときの処理（例：ログ出力）
    /// </summary>
    private void HandleOptionSubmit()
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject;
        if (currentSelected == null)
        {
            Debug.Log("Option Submit: No UI element is currently selected in OptionTabState.");
            return;
        }
        Debug.Log("Option Submit pressed on: " + currentSelected.name);
        // ここに、例えば「Apply」ボタンが選択されていれば設定確定の処理を実装する
    }
}
