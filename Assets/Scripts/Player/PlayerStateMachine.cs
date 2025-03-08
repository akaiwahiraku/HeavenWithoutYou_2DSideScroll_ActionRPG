using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState currentState { get; private set; }
    private bool isTransitionLocked = false; // �X�e�[�g�J�ڂ̃��b�N�t���O

    public void LockStateTransition(bool lockTransition)
    {
        isTransitionLocked = lockTransition;
    }

    // ���b�N��Ԃ��m�F���邽�߂̃��\�b�h
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
