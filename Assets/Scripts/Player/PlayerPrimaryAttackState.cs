using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter { get; private set; }
    private float lastTimerAttacked;
    private float comboWindow = 1.5f;

    private int damageModifier = 0; // ダメージの変更値を保存する変数
    private float attackDir; // 攻撃方向を保存する変数

    // 攻撃方向を設定するメソッド
    public void SetAttackDirection(float direction)
    {
        attackDir = direction; // 攻撃方向を受け取って設定
    }

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (comboCounter > 2 || Time.time >= lastTimerAttacked + comboWindow)
            comboCounter = 0;

        // 3撃目はダメージを1/2にして3回攻撃
        if (comboCounter == 2)
        {
            int originalDamage = player.stats.damage.GetValue();
            damageModifier = -originalDamage / 2;
            player.stats.damage.AddModifier(damageModifier);
        }

        player.anim.SetInteger("ComboCounter", comboCounter);

        // attackDir に基づいて攻撃の方向と移動を決定
        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);

        stateTimer = .1f;

        // 幻影スキル追加時
        if (SkillManager.instance.clone != null && SkillManager.instance.clone.CanUseSkill())
            player.skill.clone.CloneOnAttack(true, false);


    }

    public override void Exit()
    {
        base.Exit();

        // コンボカウンターが2の場合、ダメージの変更を解除する
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

        // 矢を跳ね返す処理
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

