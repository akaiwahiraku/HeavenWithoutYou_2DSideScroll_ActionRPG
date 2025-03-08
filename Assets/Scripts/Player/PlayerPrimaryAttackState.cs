using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter { get; private set; }
    private float lastTimerAttacked;
    private float comboWindow = 1.5f;

    private int damageModifier = 0; // �_���[�W�̕ύX�l��ۑ�����ϐ�
    private float attackDir; // �U��������ۑ�����ϐ�

    // �U��������ݒ肷�郁�\�b�h
    public void SetAttackDirection(float direction)
    {
        attackDir = direction; // �U���������󂯎���Đݒ�
    }

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (comboCounter > 2 || Time.time >= lastTimerAttacked + comboWindow)
            comboCounter = 0;

        // 3���ڂ̓_���[�W��1/2�ɂ���3��U��
        if (comboCounter == 2)
        {
            int originalDamage = player.stats.damage.GetValue();
            damageModifier = -originalDamage / 2;
            player.stats.damage.AddModifier(damageModifier);
        }

        player.anim.SetInteger("ComboCounter", comboCounter);

        // attackDir �Ɋ�Â��čU���̕����ƈړ�������
        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);

        stateTimer = .1f;

        // ���e�X�L���ǉ���
        if (SkillManager.instance.clone != null && SkillManager.instance.clone.CanUseSkill())
            player.skill.clone.CloneOnAttack(true, false);


    }

    public override void Exit()
    {
        base.Exit();

        // �R���{�J�E���^�[��2�̏ꍇ�A�_���[�W�̕ύX����������
        if (comboCounter == 2)
        {
            player.stats.damage.RemoveModifier(damageModifier);
        }

        player.StartCoroutine("BusyFor", .15f);

        comboCounter++;
        lastTimerAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            player.SetZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);

        // ��𒵂˕Ԃ�����
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Arrow_Controller>() != null)
            {
                hit.GetComponent<Arrow_Controller>().FlipArrow();
            }
        }
    }
}

