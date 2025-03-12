using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter { get; private set; }
    private float lastTimerAttacked;
    private float comboWindow = 1.5f;
    private int damageModifier = 0;
    private float attackDir;

    // 攻撃方向を設定するメソッド
    public void SetAttackDirection(float direction)
    {
        attackDir = direction;
    }

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
         : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // 攻撃開始時刻を記録（後で保持時間の判定に利用）
        //player.attackButtonPressTime = Time.time;

        // 既存のコンボやアニメーション設定（必要に応じて調整）
        if (SkillManager.instance.pyre != null && SkillManager.instance.pyre.CanUseSkill())
        {
            player.anim.speed = 0.8f;
            if (comboCounter > 1 || Time.time >= lastTimerAttacked + comboWindow)
                comboCounter = 0;
        }
        else if (SkillManager.instance.surge != null && SkillManager.instance.surge.CanUseSkill())
        {
            comboCounter = 1;
            player.anim.speed = 0.6f;
        }
        else
        {
            if (comboCounter > 2 || Time.time >= lastTimerAttacked + comboWindow)
                comboCounter = 0;
        }

        if (comboCounter == 2)
        {
            int originalDamage = player.stats.damage.GetValue();
            damageModifier = -originalDamage / 2;
            player.stats.damage.AddModifier(damageModifier);
        }

        player.anim.SetInteger("ComboCounter", comboCounter);
        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir,
                           player.attackMovement[comboCounter].y);

        // 遅延で他スキルの発動（例：Clone, Pyre, Surgeなど）
        if (SkillManager.instance.clone != null && SkillManager.instance.clone.CanUseSkill())
            player.StartCoroutine(DelayedCloneOnAttack(0.1f));
        if (SkillManager.instance.pyre != null && SkillManager.instance.pyre.CanUseSkill())
        {
            bool isSecondAttack = (comboCounter == 1);
            player.StartCoroutine(DelayedReleasePyre(0.2f, isSecondAttack));
        }
        //if (SkillManager.instance.surge != null && SkillManager.instance.surge.CanUseSkill())
        //    player.StartCoroutine(DelayedReleaseSurge(0.3f));

        SetLockStateTransition(true);
    }

    public override void Update()
    {
        base.Update();

        // 通常攻撃のアニメーション完了など、triggerCalled により攻撃終了を検知したら
        if (triggerCalled)
        {
            SetLockStateTransition(false);
            // この時点では状態は終了し、Idle状態（地上状態）へ戻る
            stateMachine.ChangeState(player.idleState);
        }

        // ※ 矢の跳ね返し処理など、既存処理はそのまま
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        foreach (var hit in colliders)
        {
            Arrow_Controller arrow = hit.GetComponent<Arrow_Controller>();
            if (arrow != null)
                arrow.FlipArrow();
        }
    }

    public override void Exit()
    {
        base.Exit();
        if (comboCounter == 2)
            player.stats.damage.RemoveModifier(damageModifier);

        if (SkillManager.instance.pyre != null && SkillManager.instance.pyre.CanUseSkill())
        {
            player.anim.speed = 1.0f;
            player.StartCoroutine("BusyFor", 0.2f);
        }
        //else if (SkillManager.instance.surge != null && SkillManager.instance.surge.CanUseSkill())
        //{
        //    player.anim.speed = 1.0f;
        //    player.StartCoroutine("BusyFor", 0.15f);
        //}
        else
            player.StartCoroutine("BusyFor", 0.15f);

        comboCounter++;
        lastTimerAttacked = Time.time;
    }

    private IEnumerator DelayedCloneOnAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (SkillManager.instance.clone != null && SkillManager.instance.clone.CanUseSkill())
            player.skill.clone.CloneOnAttack(true, false);
    }

    private IEnumerator DelayedReleasePyre(float delay, bool isSecondAttack)
    {
        yield return new WaitForSeconds(delay);
        if (SkillManager.instance.pyre != null && SkillManager.instance.pyre.CanUseSkill())
            SkillManager.instance.pyre.CreatePyre(isSecondAttack);
    }

    //private IEnumerator DelayedReleaseSurge(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    if (SkillManager.instance.surge != null && SkillManager.instance.surge.CanUseSkill())
    //        player.skill.surge.CreateSurge();
    //}
}
