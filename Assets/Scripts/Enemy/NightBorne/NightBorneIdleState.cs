using UnityEngine;

public class NightBorneIdleState : NightBorneGroundedState
{
    //private Enemy_NightBorne enemy;
    //private Transform player;
    public NightBorneIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_NightBorne enemy) : base(_enemyBase, _stateMachine, _animBoolName, enemy)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.SetZeroVelocity();

        stateTimer = enemy.idleTime;
        player = CurrencyManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.Distance(player.transform.position, enemy.transform.position) < 7)
            enemy.bossFightBegun = true;

        //if (Input.GetKeyDown(KeyCode.V))
        //    stateMachine.ChangeState(enemy.teleportState);

        if (stateTimer < 0 && enemy.bossFightBegun)
            stateMachine.ChangeState(enemy.battleState);
    }
}
