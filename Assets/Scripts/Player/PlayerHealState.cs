using System.Collections;
using UnityEngine;

public class PlayerHealState : PlayerState
{
    public PlayerHealState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // ヒールエフェクトの再生
        player.fx.PlayHealFX();

        // ステート遷移を一時的にロック（必要なら）
        SetLockStateTransition(true);

    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        if (triggerCalled)
        {
            SetLockStateTransition(false);
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        SetLockStateTransition(false);
    }
}
