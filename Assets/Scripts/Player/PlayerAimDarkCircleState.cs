public class PlayerAimDarkCircleState : PlayerState
{

    //private float defaultGravity;

    public PlayerAimDarkCircleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(13, null);

        //defaultGravity = player.rb.gravityScale;
        //rb.gravityScale = 0;
        //player.StartCoroutine("BusyFor", .5f);
        SkillManager.instance.darkCircle.UseSkill();


        // �X�e�[�g�J�ڂ����b�N
        SetLockStateTransition(true);
    }

    public override void Exit()
    {
        base.Exit();
        // �X�e�[�g�I����ɑJ�ڃ��b�N������
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
