using UnityEngine;


public class NightBorneJumpState : EnemyState
{
    private Enemy_NightBorne enemy;
    public NightBorneJumpState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_NightBorne _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        int randomDirection = Random.value < 0.5f ? 1 : -1;

        rb.velocity = new Vector2(enemy.jumpVelocity.x * randomDirection, enemy.jumpVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.anim.SetFloat("yVelocity", enemy.rb.velocity.y);

        if (rb.velocity.y < 0 && enemy.IsGroundDetectedFore() || enemy.IsGroundDetectedBack() || enemy.IsThroughGroundDetected())
            stateMachine.ChangeState(enemy.battleState);
    }
}
