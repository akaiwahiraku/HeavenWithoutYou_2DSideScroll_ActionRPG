using UnityEngine;

/// <summary>
/// UIの状態を表す抽象クラス
/// </summary>
public abstract class UIState
{
    protected UIStateMachine stateMachine;

    public UIState(UIStateMachine machine)
    {
        this.stateMachine = machine;
    }

    /// <summary>
    /// 状態が切り替わり、UIパネルをアクティブにするなどの初期化
    /// </summary>
    public virtual void Enter() { }

    /// <summary>
    /// 毎フレームの更新処理
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// 状態から抜ける際の終了処理 (UIパネルを非アクティブにするなど)
    /// </summary>
    public virtual void Exit() { }
}
