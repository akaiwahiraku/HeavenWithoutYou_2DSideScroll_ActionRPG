public class SkeletonWarriorMoveState : SkeletonWarriorGroundedState
{
    public SkeletonWarriorMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_SkeletonWarrior enemy) : base(_enemyBase, _stateMachine, _animBoolName, enemy)
    {
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

        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.velocity.y);

        if (enemy.IsWallDetected() || !enemy.IsGroundDetectedFore())
        {
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }
    }


}
