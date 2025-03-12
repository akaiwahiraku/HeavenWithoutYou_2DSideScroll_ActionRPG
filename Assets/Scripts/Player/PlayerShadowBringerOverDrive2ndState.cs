using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerShadowBringerOverDrive2ndState : PlayerState
{
    private float flyTime = .3f;
    private bool skillUsed;

    Clone_Skill_Controller attackMultiplier;
    Clone_Skill_Controller attackInBlackhole;

    private float defaultGravity;

    public PlayerStats currentOverDrive;

    PlayerStats playerStats;
    private bool onceTimestop = false;

    private GameObject foreGround;
    private TilemapRenderer foreGroundTilemapRenderer;
    private string originalSortingLayerName; // 元のレイヤー名を保存する変数


    public PlayerShadowBringerOverDrive2ndState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        playerStats = _player.GetComponent<PlayerStats>();

        // ForegroundオブジェクトとそのRendererをキャッシュ
        foreGround = GameObject.Find("Level/Grid/Foreground");
        if (foreGround != null)
        {
            foreGroundTilemapRenderer = foreGround.GetComponent<TilemapRenderer>();
            originalSortingLayerName = foreGroundTilemapRenderer.sortingLayerName;
        }
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        playerStats.isInOverdriveState = true;
        if (player.fx != null)
            player.fx.StopOverDriveFX();

        player.stats.overDriveStock = player.stats.overDriveStock - 2;

        defaultGravity = player.rb.gravityScale;
        skillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;
        player.stats.isInShadowBringerOverDrive2ndState = true;
        //attackMultiplier = attackInBlackhole;
        player.stats.MakeInvincible(true);
        onceTimestop = true;

        // Foregroundのスプライトレイヤーを「Ground」に変更
        if (foreGroundTilemapRenderer != null)
        {
            foreGroundTilemapRenderer.sortingLayerName = "Ground";
        }

        // ステート遷移をロック
        SetLockStateTransition(true);
    }


    public override void Exit()
    {
        base.Exit();

        playerStats.isInOverdriveState = false;
        if (player.stats.overDriveStock > 0 && player.fx != null)
            player.fx.PlayOverDriveFX();

        player.rb.gravityScale = defaultGravity;
        player.fx.MakeTransparent(false);
        player.stats.isInShadowBringerOverDrive2ndState = false;
        //attackInBlackhole = attackMultiplier;
        player.stats.MakeInvincible(false);

        // ForeGroundのスプライトレイヤーを元に戻す
        if (foreGroundTilemapRenderer != null)
        {
            foreGroundTilemapRenderer.sortingLayerName = originalSortingLayerName;
        }

        // ステート終了後に遷移ロックを解除
        SetLockStateTransition(false);
    }

    public override void Update()
    {
        base.Update();

        if (onceTimestop)
        {
            if (playerStats != null && playerStats.screenFlashBlackout != null)
            {
                playerStats.overDrivePhantasmNightText.FlashOverDriveText();
                playerStats.screenFlashBlackout.FlashBlackScreen();
            }
            else
            {
                Debug.LogError("PlayerStats or screenFlashBlackout is null.");
            }

            player.StartCoroutine(Restart());
            onceTimestop = false;
        }

        if (stateTimer > 0)
        {
            rb.velocity = new Vector2(0, 10);
        }

        if (stateTimer < 0)
        {
            rb.velocity = new Vector2(0, -.1f);

            if (!skillUsed)
            {
                if (player.skill.blackhole.CanUseOverDrive())
                {
                    skillUsed = true;
                    player.stats.currentOverDrive = 0;
                }

            }
        }

        if (player.skill.blackhole.SkillCompleted())
        {
            SetLockStateTransition(false);
            stateMachine.ChangeState(player.airState);
        }
    }

    public IEnumerator Restart()
    {

        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1.0f);
        Time.timeScale = 1;
    }

}

