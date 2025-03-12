using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpAttackChargeState : PlayerState
{
    private float attackDir; // �U��������ۑ�����ϐ�
    private float defaultGravity;


    // �U��������ݒ肷�郁�\�b�h
    public void SetAttackDirection(float direction)
    {
        attackDir = direction; // �U���������󂯎���Đݒ�
    }

    public PlayerJumpAttackChargeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // AudioManager.instance.PlaySFX(2); // attack sound effect

        // �����̑��x���ێ����Ȃ���U�������̈ړ���ǉ�
        AddAttackVelocity();
        defaultGravity = player.rb.gravityScale;
        //rb.gravityScale = 0;
        player.StartCoroutine(DelayedReleaseSurge(0.5f));
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
        //player.SetVelocity(finalVelocityX, currentVelocity.y);
        SetLockStateTransition(true);
    }

    public override void Exit()
    {
        base.Exit();

        // �U�����������Z�b�g���đ��̃X�e�[�g�ɉe�����Ȃ��悤�ɂ���
        attackDir = 0;
        
    }

    public override void Update()
    {
        base.Update();

        //player.SetZeroVelocity();


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
            player.rb.gravityScale = defaultGravity;
            SetLockStateTransition(false);

            stateMachine.ChangeState(player.idleState);
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