using UnityEngine;

public class SkeletonWarriorGroundedState : EnemyState
{
    protected Enemy_SkeletonWarrior enemy;

    protected Transform player;
    public SkeletonWarriorGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_SkeletonWarrior _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = CurrencyManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.position) < enemy.agroDistance)
            stateMachine.ChangeState(enemy.battleState);
    }
}
