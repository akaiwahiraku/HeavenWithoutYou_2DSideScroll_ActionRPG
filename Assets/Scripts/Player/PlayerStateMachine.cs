using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState currentState { get; private set; }
    private bool isTransitionLocked = false; // ステート遷移のロックフラグ

    public void LockStateTransition(bool lockTransition)
    {
        isTransitionLocked = lockTransition;
    }

    // ロック状態を確認するためのメソッド
    public bool IsTransitionLocked()
    {
        return isTransitionLocked;
    }

    public void Initialize(PlayerState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        if (isTransitionLocked)
        {
            Debug.Log("State transition is locked.");
            return;
        }

        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
