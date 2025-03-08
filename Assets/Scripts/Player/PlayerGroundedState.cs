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
        // Groundedに戻ったときの向きを初期化
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

        // 通常攻撃
        if (joystickInputManager.Button2Down() && !joystickInputManager.Button5())
        {
            float attackDir = horizontalInput;
            attackDir = attackDir == 0 ? player.facingDir : Mathf.Sign(attackDir); // 入力がない場合は facingDir を使用
            player.primaryAttack.SetAttackDirection(attackDir); // 攻撃方向を設定
            stateMachine.ChangeState(player.primaryAttack);
        }

        // (A) "すり抜け床" の処理を先に
        if (player.IsThroughGroundDetected() && verticalInput < 0 && joystickInputManager.Button0Down())
        {
            IgnoreThroughGroundCollision();
        }
        // (B) そのあとでジャンプ
        else if (joystickInputManager.Button0Down() &&
         !UIManager.instance.isMenuOpen &&
         !UIManager.instance.menuJustClosed &&  // ← 追加：直前にメニューが閉じられていなければ
         (player.IsGroundDetectedFore() || player.IsGroundDetectedBack() || player.IsThroughGroundDetected()))
        {
            stateMachine.ChangeState(player.jumpState);
        }


        // 特殊スキル（SkillManager の specialSkill にセットされている && スキルが解放済み）
        if (joystickInputManager.Button3Down() && !joystickInputManager.Button5())
        {
            if (SkillManager.instance.darkCircle != null && SkillManager.instance.darkCircle.CanUseSkill())
                stateMachine.ChangeState(player.aimDarkCircle);
            if (SkillManager.instance.shadowFlare != null && SkillManager.instance.shadowFlare.CanUseSkill())
                stateMachine.ChangeState(player.aimShadowFlare);
            if (SkillManager.instance.force != null && SkillManager.instance.force.CanUseSkill())
                stateMachine.ChangeState(player.aimForce);

        }   

        // 回復スキル
        if (joystickInputManager.Button4Down() &&
            SkillManager.instance.heal != null && SkillManager.instance.heal.CanUseSkill())
        {
            stateMachine.ChangeState(player.heal);
        }

        // 防御
        if (joystickInputManager.Button5() && !joystickInputManager.Button2())
        {
            stateMachine.ChangeState(player.guard);
        }

        // ダッシュの入力処理
        CheckForDashInput();

        // 空中状態への移行
        if (!player.IsGroundDetectedFore() && !player.IsGroundDetectedBack() && !player.IsThroughGroundDetected())
            stateMachine.ChangeState(player.airState);
    }

    // ダッシュ入力チェック（地上）
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

    // 一時的にThroughGroundのコライダーを無視する処理
    public void IgnoreThroughGroundCollision()
    {
        Collider2D throughGroundCollider = Physics2D.OverlapCircle(player.throughGroundCheck.position, player.throughGroundCheckDistance, player.whatIsThroughGround);
        if (throughGroundCollider != null)
        {
            Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), throughGroundCollider, true);
            player.StartCoroutine(RestoreThroughGroundCollision(throughGroundCollider));  // すり抜け後にコライダーを再有効化
        }
    }

    // すり抜けが完了した後にコライダーを再有効化する処理
    private IEnumerator RestoreThroughGroundCollision(Collider2D throughGroundCollider)
    {
        yield return new WaitForSeconds(0.5f);  // 任意の時間、コライダーを無効化
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), throughGroundCollider, false);
    }
}
