using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private float horizontalVelocity; // �������̑��x��ێ����邽�߂̕ϐ�

    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = .05f;

        // �_�b�V���X�e�[�g����̑J�ڎ���X���̑��x���_�b�V�����x�́Z�{�ɂ����l��K�p
        player.rb.velocity = new Vector2(horizontalVelocity, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();

        // X���̑��x�����Z�b�g����i�f�t�H���g�̏�Ԃɖ߂��j
        horizontalVelocity = 0; // �K�v�ɉ����ăf�t�H���g�̑��x��ݒ�
    }

    public override void Update()
    {
        base.Update();


        if (stateTimer < 0)
            stateMachine.ChangeState(player.airState);
    }

    public void SetJumpVelocity(float velocity)
    {
        horizontalVelocity = velocity; // �_�b�V�����̑��x��ۑ�
    }
}
