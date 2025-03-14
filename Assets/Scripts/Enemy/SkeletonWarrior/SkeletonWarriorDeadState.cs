using UnityEngine;

public class SkeletonWarriorDeadState : EnemyState
{
    private Enemy_SkeletonWarrior enemy;

    public SkeletonWarriorDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_SkeletonWarrior _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.stats.MakeTransparent(true);
        stateTimer = .15f;

    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 2);

        if (triggerCalled)
            enemy.SelfDestroy();
    }
}
