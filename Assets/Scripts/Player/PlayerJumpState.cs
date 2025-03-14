using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private float horizontalVelocity; // 横方向の速度を保持するための変数

    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = .05f;

        // ダッシュステートからの遷移時にX軸の速度をダッシュ速度の〇倍にした値を適用
        player.rb.velocity = new Vector2(horizontalVelocity, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();

        // X軸の速度をリセットする（デフォルトの状態に戻す）
        horizontalVelocity = 0; // 必要に応じてデフォルトの速度を設定
    }

    public override void Update()
    {
        base.Update();


        if (stateTimer < 0)
            stateMachine.ChangeState(player.airState);
    }

    public void SetJumpVelocity(float velocity)
    {
        horizontalVelocity = velocity; // ダッシュ時の速度を保存
    }
}
