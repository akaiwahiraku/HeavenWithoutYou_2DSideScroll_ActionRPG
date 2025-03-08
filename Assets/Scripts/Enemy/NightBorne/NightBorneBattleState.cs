//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class NightBorneBattleState : EnemyState
//{
//    private Transform player;
//    private Enemy_NightBorne enemy;
//    private int moveDir;
//    public NightBorneBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_NightBorne _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
//    {
//        this.enemy = _enemy;
//    }

//    public override void Enter()
//    {
//        base.Enter();

//        player = PlayerManager.instance.player.transform;

//        if (player.GetComponent<PlayerStats>().isDead)
//            stateMachine.ChangeState(enemy.idleState);

//        stateTimer = enemy.battleTime;
//    }

//    public override void Exit()
//    {
//        base.Exit();
//    }

//    public override void Update()
//    {
//        base.Update();

//        if (enemy.IsPlayerDetected())
//        {
//            stateTimer = enemy.battleTime;

//            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
//            {
//                //if (CanAttack())
//                stateMachine.ChangeState(enemy.attackState);
//                //else //近距離でステートが切り替わるバグ
//                //    stateMachine.ChangeState(enemy.idleState);
//            }
//            else if (enemy.IsPlayerDetected().distance > enemy.attackDistance)
//            {
//                stateMachine.ChangeState(enemy.spellCastState);
//            }
//            else if (stateTimer < 0 || enemy.IsPlayerDetected().distance > enemy.agroDistance)
//                stateMachine.ChangeState(enemy.idleState);

//        }


//        if (player.position.x > enemy.transform.position.x)
//            moveDir = 1;
//        else if(player.position.x < enemy.transform.position.x)
//            moveDir = -1;

//        //if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.attackDistance - .1f)
//        //    return;

//        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);

//        BattleStateFlipControll();
//    }

//    private void BattleStateFlipControll()
//    {
//        if (player.position.x > enemy.transform.position.x && enemy.facingDir == -1)
//            enemy.Flip();
//        else if (player.position.x < enemy.transform.position.x && enemy.facingDir == 1)
//            enemy.Flip();
//    }

//    //private bool CanAttack()
//    //{
//    //    if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
//    //    {
//    //        enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
//    //        enemy.lastTimeAttacked = Time.time;
//    //        return true;
//    //    }

//    //    return false;
//    //}

//}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightBorneBattleState : EnemyState
{
    private Transform player;
    private Enemy_NightBorne enemy;
    private int moveDir;
    private float safeDistance = 1.1f;

    public NightBorneBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_NightBorne _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = CurrencyManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.idleState);

        stateTimer = enemy.battleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            float distanceToPlayer = enemy.IsPlayerDetected().distance;

            
            if (distanceToPlayer <= safeDistance && CanJump())
            {
                stateMachine.ChangeState(enemy.jumpState);
            }
            else if (distanceToPlayer > safeDistance && distanceToPlayer <= enemy.attackDistance)
            {
                if (CanJump())
                    stateMachine.ChangeState(enemy.jumpState);
                else
                    stateMachine.ChangeState(enemy.attackState);
            }
            else if (distanceToPlayer > enemy.attackDistance)
            {
                stateMachine.ChangeState(enemy.spellCastState);
            }
            else if (stateTimer < 0 || distanceToPlayer > enemy.agroDistance)
                stateMachine.ChangeState(enemy.idleState);
            else
            {
                stateMachine.ChangeState(enemy.jumpState);
            }
        }

        // プレイヤーの位置に基づいて移動方向を決定
        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;

        // 一定距離以上にいる場合、プレイヤーに近づく
        if (Mathf.Abs(player.position.x - enemy.transform.position.x) > enemy.attackDistance)
        {
            enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
        }
        else
        {
            if (CanJump())
                stateMachine.ChangeState(enemy.jumpState);
            else
                stateMachine.ChangeState(enemy.attackState);
        }

        BattleStateFlipControll();
    }

    private void BattleStateFlipControll()
    {
        // プレイヤーとの距離がsafeDistanceより大きく、敵が進行方向と逆に向いている場合にFlipする
        if (Mathf.Abs(player.position.x - enemy.transform.position.x) > .1f)
        {
            if (player.position.x > enemy.transform.position.x && enemy.facingDir == -.5)
                enemy.Flip();
            else if (player.position.x < enemy.transform.position.x && enemy.facingDir == .5)
                enemy.Flip();
        }
    }

    //private bool CanAttack()
    //{
    //    if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
    //    {
    //        enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
    //        enemy.lastTimeAttacked = Time.time;
    //        return true;
    //    }

    //    return false;
    //}

    private bool CanJump()
    {

        if (Time.time >= enemy.lastTimeJumped + enemy.jumpCooldown)
        {

            enemy.lastTimeJumped = Time.time;
            return true;
        }

        return false;
    }
}
