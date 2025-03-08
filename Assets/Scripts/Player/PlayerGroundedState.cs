using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundedState : PlayerState
{
    public PlayerStats currentOverDrive;
    public PlayerStats maxOverDrive;

    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
        joystickInputManager = JoystickInputManager.Instance;
    }

    public override void Enter()
    {
        base.Enter();
        // Grounded�ɖ߂����Ƃ��̌�����������
        player.dashDir = 0;
        player.rushDir = 0;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        joystickInputManager = JoystickInputManager.Instance;
        horizontalInput = joystickInputManager.Horizontal;
        verticalInput = joystickInputManager.Vertical;

        // �ʏ�U��
        if (joystickInputManager.Button2Down() && !joystickInputManager.Button5())
        {
            float attackDir = horizontalInput;
            attackDir = attackDir == 0 ? player.facingDir : Mathf.Sign(attackDir); // ���͂��Ȃ��ꍇ�� facingDir ���g�p
            player.primaryAttack.SetAttackDirection(attackDir); // �U��������ݒ�
            stateMachine.ChangeState(player.primaryAttack);
        }

        // (A) "���蔲����" �̏�������
        if (player.IsThroughGroundDetected() && verticalInput < 0 && joystickInputManager.Button0Down())
        {
            IgnoreThroughGroundCollision();
        }
        // (B) ���̂��ƂŃW�����v
        else if (joystickInputManager.Button0Down() &&
         !UIManager.instance.isMenuOpen &&
         !UIManager.instance.menuJustClosed &&  // �� �ǉ��F���O�Ƀ��j���[�������Ă��Ȃ����
         (player.IsGroundDetectedFore() || player.IsGroundDetectedBack() || player.IsThroughGroundDetected()))
        {
            stateMachine.ChangeState(player.jumpState);
        }


        // ����X�L���iSkillManager �� specialSkill �ɃZ�b�g����Ă��� && �X�L��������ς݁j
        if (joystickInputManager.Button3Down() && !joystickInputManager.Button5())
        {
            if (SkillManager.instance.darkCircle != null && SkillManager.instance.darkCircle.CanUseSkill())
                stateMachine.ChangeState(player.aimDarkCircle);
            if (SkillManager.instance.shadowFlare != null && SkillManager.instance.shadowFlare.CanUseSkill())
                stateMachine.ChangeState(player.aimShadowFlare);
            if (SkillManager.instance.force != null && SkillManager.instance.force.CanUseSkill())
                stateMachine.ChangeState(player.aimForce);

        }   

        // �񕜃X�L��
        if (joystickInputManager.Button4Down() &&
            SkillManager.instance.heal != null && SkillManager.instance.heal.CanUseSkill())
        {
            stateMachine.ChangeState(player.heal);
        }

        // �h��
        if (joystickInputManager.Button5() && !joystickInputManager.Button2())
        {
            stateMachine.ChangeState(player.guard);
        }

        // �_�b�V���̓��͏���
        CheckForDashInput();

        // �󒆏�Ԃւ̈ڍs
        if (!player.IsGroundDetectedFore() && !player.IsGroundDetectedBack() && !player.IsThroughGroundDetected())
            stateMachine.ChangeState(player.airState);
    }

    // �_�b�V�����̓`�F�b�N�i�n��j
    protected void CheckForDashInput()
    {
        if (player.IsWallDetected())
            return;

        if (joystickInputManager.Button1Down() && !joystickInputManager.Button5() &&
            (player.IsGroundDetectedFore() || player.IsGroundDetectedBack() || player.IsThroughGroundDetected()))
        {
            player.dashDir = horizontalInput;
            player.dashDir = player.dashDir == 0 ? player.facingDir : player.dashDir;
            stateMachine.ChangeState(player.dashState);
        }
    }

    // �ꎞ�I��ThroughGround�̃R���C�_�[�𖳎����鏈��
    public void IgnoreThroughGroundCollision()
    {
        Collider2D throughGroundCollider = Physics2D.OverlapCircle(player.throughGroundCheck.position, player.throughGroundCheckDistance, player.whatIsThroughGround);
        if (throughGroundCollider != null)
        {
            Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), throughGroundCollider, true);
            player.StartCoroutine(RestoreThroughGroundCollision(throughGroundCollider));  // ���蔲����ɃR���C�_�[���ėL����
        }
    }

    // ���蔲��������������ɃR���C�_�[���ėL�������鏈��
    private IEnumerator RestoreThroughGroundCollision(Collider2D throughGroundCollider)
    {
        yield return new WaitForSeconds(0.5f);  // �C�ӂ̎��ԁA�R���C�_�[�𖳌���
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), throughGroundCollider, false);
    }
}
