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

        // 壁から離れた場合、空中状態に遷移
        if (!player.IsWallDetected())
        {
            stateMachine.ChangeState(player.airState);
            return;
        }

        // ジャンプボタンでウォールジャンプに遷移
        if (joystickInputManager.Button0Down())
        {
            stateMachine.ChangeState(player.wallJump);
            return;
        }

        // 壁を滑りながら反対方向への移動でアイドル状態に遷移
        float xInput = joystickInputManager.Horizontal;
        if (xInput != 0 && player.facingDir != Mathf.Sign(xInput))
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // 下方向の入力がある場合、速度を調整
        float yInput = joystickInputManager.Vertical;
        if (yInput < 0)
        {
            player.rb.velocity = new Vector2(0, player.rb.velocity.y);
        }
        else
        {
            player.rb.velocity = new Vector2(0, player.rb.velocity.y * 0.7f);
        }

        // 地面に着地した場合、アイドル状態に遷移
        if (player.IsGroundDetectedFore())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
