public class PlayerAimForceState : PlayerState
{

    private float defaultGravity;

    public PlayerAimForceState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(13, null);
        player.fx.PlayStartForceFX();

        defaultGravity = player.rb.gravityScale;
        rb.gravityScale = 0;
        //player.StartCoroutine("BusyFor", .5f);
        SkillManager.instance.force.UseSkill();

        player.stats.MakeInvincible(true);
        // ステート遷移をロック
        SetLockStateTransition(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.stats.MakeInvincible(false);

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

            player.rb.gravityScale = defaultGravity;
            stateMachine.ChangeState(player.idleState);
        }
    }


}
