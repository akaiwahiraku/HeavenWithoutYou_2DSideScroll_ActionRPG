using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackChargeState : PlayerState
{
    //private float chargeTimer = 0f;
    //private float chargeSpeed = 10f;
    //private bool chargedAttackReady = false;
    //// チャージ遅延時間（この時間経過後に移動開始）
    //private float chargeDelay = 0.3f;
    //private float attackDirection;

    private float defaultGravity;
    private float attackDir;
    private int armorModifier; // このステート中に追加するarmorのモディファイア


    // 攻撃方向を設定するメソッド
    public void SetAttackDirection(float direction)
    {
        attackDir = direction;
    }

    public PlayerPrimaryAttackChargeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        SetLockStateTransition(true);

        player.StartCoroutine(DelayedReleaseSurge(0.5f));

        player.SetVelocity(attackDir,0);

        // このステートに入っている間、armor を 2 倍にする
        // 現在のarmor値を取得して同じ数値をモディファイアとして追加
        armorModifier = player.stats.armor.GetValue();
        player.stats.armor.AddModifier(armorModifier);

        // チャージタイマーを初期化
        //chargeTimer = 0f;
        //player.rb.velocity = new Vector2(player.facingDir, 2f);

        //chargedAttackReady = false;
        // ここではすぐには移動させず、チャージ演出（アニメーションなど）を再生可能
        // ※ なお、Enter() で速度を変更せずに Update() で後から移動させるようにする
        //player.SetZeroVelocity();

        //defaultGravity = player.rb.gravityScale;
        //rb.gravityScale = 0;
    }

    public override void Update()
    {
        base.Update();
        //chargeTimer += Time.deltaTime;
        //player.SetVelocity(chargeSpeed * player.facingDir, 0f);

        // 一定時間（chargeDelay）経過したら、まだ移動していなければ移動を開始する
        //if (!chargedAttackReady && chargeTimer >= chargeDelay)
        //{
        //    chargedAttackReady = true;
        //    // ここで、プレイヤーが向いている方向に一定速度で移動させる
        //    // player.facingDir が -1 または 1 を示している前提です
        //    //float chargeSpeed = chargeSpeed; // 例えば、Player クラスに定義しておくか、定数として指定
        //    player.rb.velocity = new Vector2(player.facingDir * chargeSpeed, player.rb.velocity.y);
        //}


        // triggerCalled などでこの状態を終了させるタイミングが来たら IdleState へ遷移
        if (triggerCalled)
        {
            //player.rb.gravityScale = defaultGravity;

            SetLockStateTransition(false);
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", 0.2f);
        // 必要に応じて、エフェクトやパラメータのリセット処理を実施
    }

    private IEnumerator DelayedReleaseSurge(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (SkillManager.instance.surge != null && SkillManager.instance.surge.CanUseSkill())
            player.skill.surge.CreateSurge();
    }

    // 攻撃方向を設定するメソッド（必要に応じて利用）
    //public void SetAttackDirection(float direction)
    //{
    //    attackDirection = direction;
    //}

    //// 押下時間を受け取るメソッド（利用例に合わせて）
    //public void SetChargeTime(float time)
    //{
    //    chargeTimer = time;
    //}
}
