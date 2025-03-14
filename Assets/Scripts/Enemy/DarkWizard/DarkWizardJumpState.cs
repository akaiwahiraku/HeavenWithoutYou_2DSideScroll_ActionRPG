using UnityEngine;


public class DarkWizardJumpState : EnemyState
{
    private Enemy_DarkWizard enemy;
    public DarkWizardJumpState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DarkWizard _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        rb.velocity = new Vector2(enemy.jumpVelocity.x * -enemy.facingDir, enemy.jumpVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.anim.SetFloat("yVelocity", enemy.rb.velocity.y);

        if (rb.velocity.y < 0 && enemy.IsGroundDetectedFore())
            stateMachine.ChangeState(enemy.battleState);
    }
}
