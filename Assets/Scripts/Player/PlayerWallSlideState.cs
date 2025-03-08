using System.Collections;
using UnityEngine;

public class PlayerWallSlideState : PlayerState
{

    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        joystickInputManager = JoystickInputManager.Instance;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // �ǂ��痣�ꂽ�ꍇ�A�󒆏�ԂɑJ��
        if (!player.IsWallDetected())
        {
            stateMachine.ChangeState(player.airState);
            return;
        }

        // �W�����v�{�^���ŃE�H�[���W�����v�ɑJ��
        if (joystickInputManager.Button0Down())
        {
            stateMachine.ChangeState(player.wallJump);
            return;
        }

        // �ǂ�����Ȃ��甽�Ε����ւ̈ړ��ŃA�C�h����ԂɑJ��
        float xInput = joystickInputManager.Horizontal;
        if (xInput != 0 && player.facingDir != Mathf.Sign(xInput))
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // �������̓��͂�����ꍇ�A���x�𒲐�
        float yInput = joystickInputManager.Vertical;
        if (yInput < 0)
        {
            player.rb.velocity = new Vector2(0, player.rb.velocity.y);
        }
        else
        {
            player.rb.velocity = new Vector2(0, player.rb.velocity.y * 0.7f);
        }

        // �n�ʂɒ��n�����ꍇ�A�A�C�h����ԂɑJ��
        if (player.IsGroundDetectedFore())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
