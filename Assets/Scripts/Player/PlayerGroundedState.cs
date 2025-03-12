using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundedState : PlayerState
{
    public PlayerStats currentOverDrive;
    public PlayerStats maxOverDrive;

    // 地上状態での追加：チャージ遷移用の閾値
    //private float chargeTransitionThreshold = 1.0f; // 例：1秒以上保持でチャージ攻撃へ

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

        // 攻撃ボタンが新たに押されたときは通常攻撃状態に遷移（この部分は既存）
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
                // PrimaryAttackState に入る際に、攻撃開始時刻を記録（Player.attackButtonPressTime を Player クラスに定義しておく）
                //player.attackButtonPressTime = Time.time;
                float attackDir = horizontalInput;
                attackDir = (attackDir == 0 ? player.facingDir : Mathf.Sign(attackDir));
                player.primaryAttack.SetAttackDirection(attackDir);
                stateMachine.ChangeState(player.primaryAttack);
                return;

            }
        }

        // (A) すり抜け床の処理、(B) ジャンプ処理
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

        //スペシャルスキル
        if (joystickInputManager.Button3Down() && !joystickInputManager.Button5())
        {
            if (SkillManager.instance.darkCircle != null && SkillManager.instance.darkCircle.CanUseSkill())
                stateMachine.ChangeState(player.aimDarkCircle);
            if (SkillManager.instance.shadowFlare != null && SkillManager.instance.shadowFlare.CanUseSkill())
                stateMachine.ChangeState(player.aimShadowFlare);
            if (SkillManager.instance.force != null && SkillManager.instance.force.CanUseSkill())
                stateMachine.ChangeState(player.aimForce);
        }

        //ヒール
        if (joystickInputManager.Button4Down() &&
            SkillManager.instance.heal != null && SkillManager.instance.heal.CanUseSkill())
        {
            stateMachine.ChangeState(player.heal);
        }

        //ガード
        if (joystickInputManager.Button5() && !joystickInputManager.Button2())
        {
            stateMachine.ChangeState(player.guard);
        }

        //ダッシュ
        CheckForDashInput();

        //空中への移行
        if (!player.IsGroundDetectedFore() && !player.IsGroundDetectedBack() && !player.IsThroughGroundDetected())
            stateMachine.ChangeState(player.airState);

        // チャージ攻撃処理。攻撃ボタンが離されたタイミングで、記録された押下開始時刻があれば処理する
        //if (player.attackButtonPressTime > 0 && !joystickInputManager.Button2())
        //{
        //    float holdDuration = Time.time - player.attackButtonPressTime;
        //    // 一定時間以上保持されていたならチャージ攻撃状態に遷移
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
        //        // 押下時間が短い場合は記録をリセットする
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
