public class PlayerHealState : PlayerState
{
    public PlayerHealState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // �q�[���G�t�F�N�g�̍Đ�
        player.fx.PlayHealFX();

        // �X�e�[�g�J�ڂ��ꎞ�I�Ƀ��b�N�i�K�v�Ȃ�j
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
