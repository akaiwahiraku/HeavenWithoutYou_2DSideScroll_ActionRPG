using UnityEngine;

/// <summary>
/// UI の状態を管理するステートマシン
/// </summary>
public class UIStateMachine : MonoBehaviour
{
    // 現在の状態
    private UIState currentState;

    // 毎フレーム現在の状態の Update を呼ぶ
    private void Update()
    {
        currentState?.Update();
    }

    /// <summary>
    /// 状態を切り替えるメソッド
    /// </summary>
    public void ChangeState(UIState newState)
    {
        // いまの状態を Exit
        currentState?.Exit();

        // 新しい状態に切り替える
        currentState = newState;

        // 新しい状態の Enter
        currentState?.Enter();
    }
}
