using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpAttackState : PlayerState
{
    private float attackDir; // 攻撃方向を保存する変数

    // 攻撃方向を設定するメソッド
    public void SetAttackDirection(float direction)
    {
        attackDir = direction; // 攻撃方向を受け取って設定
    }

    public PlayerJumpAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // AudioManager.instance.PlaySFX(2); // attack sound effect

        // 既存の速度を維持しながら攻撃方向の移動を追加
        AddAttackVelocity();

        // クローンアタックスキルの発動
        if (SkillManager.instance.clone != null && SkillManager.instance.clone.CanUseSkill())
        {
            // 例として0.2秒遅延させる場合
            player.StartCoroutine(DelayedCloneOnAttack(0.1f));
        }

        // 火葬スキル
        if (SkillManager.instance.pyre != null && SkillManager.instance.pyre.CanUseSkill())
        {
            // 例として0.2秒遅延させる場合
            player.StartCoroutine(DelayedReleasePyre(0.2f));
        }


    }

    // 攻撃方向に基づく速度加算処理
    private void AddAttackVelocity()
    {
        // 現在の速度を取得
        Vector2 currentVelocity = player.rb.velocity;

        // 横方向の速度を、攻撃方向に基づいて変更（既存の速度を完全に維持）
        float adjustedVelocityX = player.attackMovement[0].x * attackDir; // attackDir を使用
        float finalVelocityX = adjustedVelocityX != 0 ? adjustedVelocityX : currentVelocity.x;


        // 縦方向の速度は維持
        player.SetVelocity(finalVelocityX, currentVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        // 攻撃方向をリセットして他のステートに影響しないようにする
        attackDir = 0;
    }

    public override void Update()
    {
        base.Update();

        // 矢を跳ね返す処理
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Arrow_Controller>() != null)
            {
                hit.GetComponent<Arrow_Controller>().FlipArrow();
            }
        }

        // トリガーが発動した場合、空中状態に遷移
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.airState);
        }
    }

    private IEnumerator DelayedCloneOnAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        // ステート中にまだ条件が合致しているかなど、必要に応じてチェックしてください
        if (SkillManager.instance.clone != null && SkillManager.instance.clone.CanUseSkill())
        {
            player.skill.clone.CloneOnAttack(true, false);
        }
    }

    private IEnumerator DelayedReleasePyre(float delay)
    {
        yield return new WaitForSeconds(delay);
        // ステート中にまだ条件が合致しているかなど、必要に応じてチェックしてください
        if (SkillManager.instance.pyre != null && SkillManager.instance.pyre.CanUseSkill())
        {
            SkillManager.instance.pyre.CreatePyre();
        }
    }
}
