using System.Collections;
using UnityEngine;

public class PlayerJumpAttackState : PlayerState
{
    private float attackDir; // �U��������ۑ�����ϐ�

    // �U��������ݒ肷�郁�\�b�h
    public void SetAttackDirection(float direction)
    {
        attackDir = direction; // �U���������󂯎���Đݒ�
    }

    public PlayerJumpAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // AudioManager.instance.PlaySFX(2); // attack sound effect

        // �����̑��x���ێ����Ȃ���U�������̈ړ���ǉ�
        AddAttackVelocity();

        // �N���[���A�^�b�N�X�L���̔���
        if (SkillManager.instance.clone != null && SkillManager.instance.clone.CanUseSkill())
        {
            player.StartCoroutine(DelayedCloneOnAttack(0.1f));
        }

        // �Α��X�L��
        if (SkillManager.instance.pyre != null && SkillManager.instance.pyre.CanUseSkill())
        {
            player.anim.speed = 0.8f;
            player.StartCoroutine(DelayedReleasePyre(0.2f));
        }

        // �T�[�W�X�L��
        if (SkillManager.instance.surge != null && SkillManager.instance.surge.CanUseSkill())
        {
            player.anim.speed = 0.75f;
            player.StartCoroutine(DelayedReleaseSurge(0.25f));
        }
    }

    // �U�������Ɋ�Â����x���Z����
    private void AddAttackVelocity()
    {
        // ���݂̑��x���擾
        Vector2 currentVelocity = player.rb.velocity;

        // �������̑��x���A�U�������Ɋ�Â��ĕύX�i�����̑��x�����S�Ɉێ��j
        float adjustedVelocityX = player.attackMovement[0].x * attackDir; // attackDir ���g�p
        float finalVelocityX = adjustedVelocityX != 0 ? adjustedVelocityX : currentVelocity.x;


        // �c�����̑��x�͈ێ�
        player.SetVelocity(finalVelocityX, currentVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        if (SkillManager.instance.surge != null && SkillManager.instance.surge.CanUseSkill())
        {
            player.anim.speed = 1.0f;
            //player.StartCoroutine("BusyFor", 0.2f);
        }

        // �U�����������Z�b�g���đ��̃X�e�[�g�ɉe�����Ȃ��悤�ɂ���
        attackDir = 0;
    }

    public override void Update()
    {
        base.Update();

        // ��𒵂˕Ԃ�����
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Arrow_Controller>() != null)
            {
                hit.GetComponent<Arrow_Controller>().FlipArrow();
            }
        }

        // �g���K�[�����������ꍇ�A�󒆏�ԂɑJ��
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.airState);
        }
    }

    private IEnumerator DelayedCloneOnAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        // �X�e�[�g���ɂ܂����������v���Ă��邩�ȂǁA�K�v�ɉ����ă`�F�b�N���Ă�������
        if (SkillManager.instance.clone != null && SkillManager.instance.clone.CanUseSkill())
        {
            player.skill.clone.CloneOnAttack(true, false);
        }
    }

    private IEnumerator DelayedReleasePyre(float delay)
    {
        yield return new WaitForSeconds(delay);
        // �X�e�[�g���ɂ܂����������v���Ă��邩�ȂǁA�K�v�ɉ����ă`�F�b�N���Ă�������
        if (SkillManager.instance.pyre != null && SkillManager.instance.pyre.CanUseSkill())
        {
            SkillManager.instance.pyre.CreatePyre();
        }
    }
    private IEnumerator DelayedReleaseSurge(float delay)
    {
        yield return new WaitForSeconds(delay);
        // �X�e�[�g���ɂ܂����������v���Ă��邩�ȂǁA�K�v�ɉ����ă`�F�b�N���Ă�������
        if (SkillManager.instance.surge != null && SkillManager.instance.surge.CanUseSkill())
        {
            SkillManager.instance.surge.CreateSurge();
        }
    }
}