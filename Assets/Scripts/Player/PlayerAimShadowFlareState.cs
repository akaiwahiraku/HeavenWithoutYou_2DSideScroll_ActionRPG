using System;
using System.Collections;
using UnityEngine;

public class PlayerAimShadowFlareState : PlayerState
{

    //private float defaultGravity;

    public PlayerAimShadowFlareState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(13, null);

        //defaultGravity = player.rb.gravityScale;
        //rb.gravityScale = 0;
        //player.StartCoroutine("BusyFor", .5f);
        SkillManager.instance.shadowFlare.UseSkill();


        // ステート遷移をロック
        SetLockStateTransition(true);
    }

    public override void Exit()
    {
        base.Exit();
        // ステート終了後に遷移ロックを解除
        SetLockStateTransition(false);
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();
        //player.StartCoroutine("BusyFor",.5f);

        if (triggerCalled)
        {
            SetLockStateTransition(false);

            //player.rb.gravityScale = defaultGravity;
            stateMachine.ChangeState(player.idleState);
        }
    }

    
}
