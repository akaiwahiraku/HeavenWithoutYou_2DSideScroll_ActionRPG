using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkWizardDeadState : EnemyState
{
    private Enemy_DarkWizard enemy;

    public DarkWizardDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DarkWizard _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
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
            rb.velocity = new Vector2(0, 10);

        if (triggerCalled)
            enemy.SelfDestroy();
    }
}
