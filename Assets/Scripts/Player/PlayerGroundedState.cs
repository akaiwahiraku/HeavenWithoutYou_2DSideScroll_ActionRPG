using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundedState : PlayerState
{
    public PlayerStats currentOverDrive;
    public PlayerStats maxOverDrive;

    // �n���Ԃł̒ǉ��F�`���[�W�J�ڗp��臒l
    //private float chargeTransitionThreshold = 1.0f; // ��F1�b�ȏ�ێ��Ń`���[�W�U����

    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
        joystickInputManager = JoystickInputManager.Instance;
    }

    public override void Enter()
    {
        base.Enter();
        player.dashDir = 0;
        player.rushDir = 0;
        canDoubleJump = true;
        player.canAirDash = true;

    }

    public override void Update()
    {
        base.Update();

        joystickInputManager = JoystickInputManager.Instance;
        horizontalInput = joystickInputManager.Horizontal;
        verticalInput = joystickInputManager.Vertical;

        // �U���{�^�����V���ɉ����ꂽ�Ƃ��͒ʏ�U����ԂɑJ�ځi���̕����͊����j
        if (joystickInputManager.Button2Down() && !joystickInputManager.Button5())
        {
            if (SkillManager.instance.surge != null && SkillManager.instance.surge.CanUseSkill())
            {
                float attackDir = horizontalInput;
                attackDir = (attackDir == 0 ? player.facingDir : Mathf.Sign(attackDir));
                player.primaryAttackCharge.SetAttackDirection(attackDir);
                stateMachine.ChangeState(player.primaryAttackCharge);
            }
            else
            {
                // PrimaryAttackState �ɓ���ۂɁA�U���J�n�������L�^�iPlayer.attackButtonPressTime �� Player �N���X�ɒ�`���Ă����j
                //player.attackButtonPressTime = Time.time;
                float attackDir = horizontalInput;
                attackDir = (attackDir == 0 ? player.facingDir : Mathf.Sign(attackDir));
                player.primaryAttack.SetAttackDirection(attackDir);
                stateMachine.ChangeState(player.primaryAttack);
                return;

            }
        }

        // (A) ���蔲�����̏����A(B) �W�����v����
        if (player.IsThroughGroundDetected() && verticalInput < 0 && joystickInputManager.Button0Down())
        {
            IgnoreThroughGroundCollision();
        }
        else if (joystickInputManager.Button0Down() &&
                 !UIManager.instance.isMenuOpen &&
                 !UIManager.instance.menuJustClosed &&
                 (player.IsGroundDetectedFore() || player.IsGroundDetectedBack() || player.IsThroughGroundDetected()))
        {
            stateMachine.ChangeState(player.jumpState);
        }

        //�X�y�V�����X�L��
        if (joystickInputManager.Button3Down() && !joystickInputManager.Button5())
        {
            if (SkillManager.instance.darkCircle != null && SkillManager.instance.darkCircle.CanUseSkill())
                stateMachine.ChangeState(player.aimDarkCircle);
            if (SkillManager.instance.shadowFlare != null && SkillManager.instance.shadowFlare.CanUseSkill())
                stateMachine.ChangeState(player.aimShadowFlare);
            if (SkillManager.instance.force != null && SkillManager.instance.force.CanUseSkill())
                stateMachine.ChangeState(player.aimForce);
        }

        //�q�[��
        if (joystickInputManager.Button4Down() &&
            SkillManager.instance.heal != null && SkillManager.instance.heal.CanUseSkill())
        {
            stateMachine.ChangeState(player.heal);
        }

        //�K�[�h
        if (joystickInputManager.Button5() && !joystickInputManager.Button2())
        {
            stateMachine.ChangeState(player.guard);
        }

        //�_�b�V��
        CheckForDashInput();

        //�󒆂ւ̈ڍs
        if (!player.IsGroundDetectedFore() && !player.IsGroundDetectedBack() && !player.IsThroughGroundDetected())
            stateMachine.ChangeState(player.airState);

        // �`���[�W�U�������B�U���{�^���������ꂽ�^�C�~���O�ŁA�L�^���ꂽ�����J�n����������Ώ�������
        //if (player.attackButtonPressTime > 0 && !joystickInputManager.Button2())
        //{
        //    float holdDuration = Time.time - player.attackButtonPressTime;
        //    // ��莞�Ԉȏ�ێ�����Ă����Ȃ�`���[�W�U����ԂɑJ��
        //    if (holdDuration >= chargeTransitionThreshold)
        //    {
        //        float attackDir = horizontalInput;
        //        attackDir = (attackDir == 0 ? player.facingDir : Mathf.Sign(attackDir));
        //        player.primaryAttackCharge.SetAttackDirection(attackDir);
        //        player.primaryAttackCharge.SetChargeTime(holdDuration);
        //        player.attackButtonPressTime = 0;
        //        stateMachine.ChangeState(player.primaryAttackCharge);
        //        return;
        //    }
        //    else
        //    {
        //        // �������Ԃ��Z���ꍇ�͋L�^�����Z�b�g����
        //        player.attackButtonPressTime = 0;
        //    }
        //}
    }

    protected void CheckForDashInput()
    {
        if (player.IsWallDetected())
            return;

        if (joystickInputManager.Button1Down() && !joystickInputManager.Button5() &&
            (player.IsGroundDetectedFore() || player.IsGroundDetectedBack() || player.IsThroughGroundDetected()))
        {
            player.dashDir = horizontalInput;
            player.dashDir = (player.dashDir == 0 ? player.facingDir : player.dashDir);
            stateMachine.ChangeState(player.dashState);
        }
    }

    public void IgnoreThroughGroundCollision()
    {
        Collider2D throughGroundCollider = Physics2D.OverlapCircle(player.throughGroundCheck.position, player.throughGroundCheckDistance, player.whatIsThroughGround);
        if (throughGroundCollider != null)
        {
            Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), throughGroundCollider, true);
            player.StartCoroutine(RestoreThroughGroundCollision(throughGroundCollider));
        }
    }

    private IEnumerator RestoreThroughGroundCollision(Collider2D throughGroundCollider)
    {
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), throughGroundCollider, false);
    }
}
